namespace WibboEmulator.Games.Moderations;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Moderation;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public class ModerationManager
{
    private readonly List<ModerationTicket> _tickets;
    private readonly List<string> _userMessagePresets;
    private readonly List<string> _roomMessagePresets;

    private readonly List<ModerationPresetActionMessages> _ticketResolution1;
    private readonly List<ModerationPresetActionMessages> _ticketResolution2;

    private readonly Dictionary<int, string> _moderationCFHTopics;

    private readonly Dictionary<int, List<ModerationPresetActions>> _moderationCFHTopicActions;

    public ModerationManager()
    {
        this._tickets = new List<ModerationTicket>();
        this._userMessagePresets = new List<string>();
        this._roomMessagePresets = new List<string>();
        this._ticketResolution1 = new List<ModerationPresetActionMessages>();
        this._ticketResolution2 = new List<ModerationPresetActionMessages>();
        this._moderationCFHTopics = new Dictionary<int, string>();
        this._moderationCFHTopicActions = new Dictionary<int, List<ModerationPresetActions>>();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this.LoadMessageTopics(dbClient);
        this.LoadMessagePresets(dbClient);
        this.LoadPendingTickets(dbClient);
        this.LoadTicketResolution(dbClient);
    }

    public Dictionary<string, List<ModerationPresetActions>> UserActionPresets
    {
        get
        {
            var result = new Dictionary<string, List<ModerationPresetActions>>();
            foreach (var category in this._moderationCFHTopics.ToList())
            {
                result.Add(category.Value, new List<ModerationPresetActions>());

                if (this._moderationCFHTopicActions.TryGetValue(category.Key, out var value))
                {
                    foreach (var data in value)
                    {
                        result[category.Value].Add(data);
                    }
                }
            }
            return result;
        }
    }

    public void LoadMessageTopics(IQueryAdapter dbClient)
    {
        var moderationTopics = ModerationTopicDao.GetAll(dbClient);

        if (moderationTopics != null)
        {
            foreach (DataRow row in moderationTopics.Rows)
            {
                if (!this._moderationCFHTopics.ContainsKey(Convert.ToInt32(row["id"])))
                {
                    this._moderationCFHTopics.Add(Convert.ToInt32(row["id"]), Convert.ToString(row["caption"]));
                }
            }
        }

        var moderationTopicsActions = ModerationTopicActionDao.GetAll(dbClient);

        if (moderationTopicsActions != null)
        {
            foreach (DataRow row in moderationTopicsActions.Rows)
            {
                var parentId = Convert.ToInt32(row["parent_id"]);

                if (!this._moderationCFHTopicActions.ContainsKey(parentId))
                {
                    this._moderationCFHTopicActions.Add(parentId, new List<ModerationPresetActions>());
                }

                this._moderationCFHTopicActions[parentId].Add(new ModerationPresetActions(Convert.ToInt32(row["id"]), Convert.ToInt32(row["parent_id"]), Convert.ToString(row["type"]), Convert.ToString(row["caption"]), Convert.ToString(row["message_text"]),
                    Convert.ToInt32(row["mute_time"]), Convert.ToInt32(row["ban_time"]), Convert.ToInt32(row["ip_time"]), Convert.ToInt32(row["trade_lock_time"]), Convert.ToString(row["default_sanction"])));
            }
        }
    }

    public void LoadMessagePresets(IQueryAdapter dbClient)
    {
        this._userMessagePresets.Clear();
        this._roomMessagePresets.Clear();

        var table = ModerationPresetDao.GetAll(dbClient);
        foreach (DataRow dataRow in table.Rows)
        {
            var message = dataRow["message"].ToString();
            var type = dataRow["type"].ToString();

            switch (type?.ToLower())
            {
                case "message":
                    this._userMessagePresets.Add(message);
                    continue;
                case "roommessage":
                    this._roomMessagePresets.Add(message);
                    continue;
                default:
                    continue;
            }
        }
    }

    public void LoadTicketResolution(IQueryAdapter dbClient)
    {
        this._ticketResolution1.Clear();
        this._ticketResolution2.Clear();

        var table = ModerationResolutionDao.GetAll(dbClient);
        foreach (DataRow dataRow in table.Rows)
        {
            var str = new ModerationPresetActionMessages((string)dataRow["title"], (string)dataRow["subtitle"], Convert.ToInt32(dataRow["ban_hours"]), Convert.ToInt32(dataRow["enable_mute"]), Convert.ToInt32(dataRow["mute_hours"]), Convert.ToInt32(dataRow["reminder"]), (string)dataRow["message"]);
            switch (dataRow["type"].ToString())
            {
                case "Sexual":
                    this._ticketResolution1.Add(str);
                    continue;
                case "PII":
                    this._ticketResolution2.Add(str);
                    continue;
                default:
                    continue;
            }
        }
    }

    public void LoadPendingTickets(IQueryAdapter dbClient)
    {
        var table = ModerationTicketDao.GetAll(dbClient);
        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            var moderationTicket = new ModerationTicket(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["score"]), Convert.ToInt32(dataRow["type"]), Convert.ToInt32(dataRow["sender_id"]), Convert.ToInt32(dataRow["reported_id"]), (string)dataRow["message"], Convert.ToInt32(dataRow["room_id"]), (string)dataRow["room_name"], (int)dataRow["timestamp"]);

            var status = dataRow["status"].ToString();
            if (status?.ToLower() == "picked")
            {
                moderationTicket.Pick(Convert.ToInt32(dataRow["moderator_id"]), false);
            }

            this._tickets.Add(moderationTicket);
        }
    }

    public void SendNewTicket(GameClient session, int category, int reportedUser, string message)
    {
        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(session.User.CurrentRoomId);
        var id = 0;
        var roomname = (roomData != null) ? roomData.Name : "Aucun appart";
        var roomId = (roomData != null) ? roomData.Id : 0;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            id = ModerationTicketDao.Insert(dbClient, message, roomname, category, session.User.Id, reportedUser, roomId);
        }

        var ticket = new ModerationTicket(id, 1, category, session.User.Id, reportedUser, message, roomId, roomname, WibboEnvironment.GetUnixTimestamp());
        this._tickets.Add(ticket);
        SendTicketToModerators(ticket);
    }

    public static void ApplySanction(GameClient session, int reportedUser)
    {
        if (reportedUser == 0)
        {
            return;
        }

        var userReport = WibboEnvironment.GetUserById(reportedUser);
        if (userReport == null)
        {
            return;
        }

        session.User.Messenger.DestroyFriendship(userReport.Id);

        session.SendPacket(new IgnoreStatusComposer(1, userReport.Username));

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        if ((room.RoomData.BanFuse != 1 || !room.CheckRights(session)) && !room.CheckRights(session, true))
        {
            return;
        }

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByUserId(userReport.Id);
        if (roomUserByUserId == null || roomUserByUserId.IsBot || room.CheckRights(roomUserByUserId.Client, true) || roomUserByUserId.Client.User.HasPermission("mod") || roomUserByUserId.Client.User.HasPermission("no_kick"))
        {
            return;
        }

        room.AddBan(userReport.Id, 429496729);
        room.RoomUserManager.RemoveUserFromRoom(roomUserByUserId.Client, true, true);
    }

    public ModerationTicket GetTicket(int ticketId)
    {
        foreach (var moderationTicket in this._tickets)
        {
            if (moderationTicket.TicketId == ticketId)
            {
                return moderationTicket;
            }
        }
        return null;
    }

    public List<string> UserMessagePresets() => this._userMessagePresets;

    public List<ModerationTicket> Tickets() => this._tickets;

    public List<string> RoomMessagePresets() => this._roomMessagePresets;

    public void PickTicket(GameClient session, int ticketId)
    {
        var ticket = this.GetTicket(ticketId);
        if (ticket == null || ticket.Status != TicketStatusType.Open)
        {
            return;
        }

        ticket.Pick(session.User.Id, true);
        SendTicketToModerators(ticket);
    }

    public void ReleaseTicket(GameClient session, int ticketId)
    {
        var ticket = this.GetTicket(ticketId);
        if (ticket == null || ticket.Status != TicketStatusType.Picked || ticket.ModeratorId != session.User.Id)
        {
            return;
        }

        ticket.Release(true);
        SendTicketToModerators(ticket);
    }

    public void CloseTicket(GameClient session, int ticketId, int result)
    {
        var ticket = this.GetTicket(ticketId);
        if (ticket == null || ticket.Status != TicketStatusType.Picked || ticket.ModeratorId != session.User.Id)
        {
            return;
        }

        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(ticket.SenderId);

        TicketStatusType newStatus;
        string messageAlert;
        switch (result)
        {
            case 1:
                newStatus = TicketStatusType.Invalid;
                messageAlert = "Es-tu certain d'avoir bien utilisé cet outil ? Nous voulons donner le meilleur des services mais nous devons aussi aider d'autres personnes dans l'urgence...";
                break;
            case 2:
                newStatus = TicketStatusType.Abusive;
                messageAlert = "Merci de ne pas utiliser l'outil d'appel à l'aide pour rien. Tu risques l'exclusion.";
                break;
            default:
                newStatus = TicketStatusType.Resolved;
                messageAlert = "Merci, ton souci est résolu ou en cours de résolution. N'hésite pas à Ignorer la personne  ou à la supprimer de ta console s'il s'agit d'insultes.";
                break;
        }
        clientByUserId?.SendPacket(new ModeratorSupportTicketResponseComposer(messageAlert));
        ticket.Close(newStatus, true);
        SendTicketToModerators(ticket);
    }

    public bool UsersHasPendingTicket(int id)
    {
        foreach (var moderationTicket in this._tickets)
        {
            if (moderationTicket.SenderId == id && moderationTicket.Status == TicketStatusType.Open)
            {
                return true;
            }
        }
        return false;
    }

    public void DeletePendingTicketForUser(int id)
    {
        foreach (var ticket in this._tickets)
        {
            if (ticket.SenderId == id)
            {
                ticket.Delete(true);
                SendTicketToModerators(ticket);
                break;
            }
        }
    }

    public static void SendTicketToModerators(ModerationTicket ticket) => WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(new ModeratorSupportTicketComposer(ticket));

    public static void LogStaffEntry(int userId, string modName, int roomId, string target, string type, string description)
    {
        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        LogCommandDao.Insert(dbClient, userId, modName, roomId, target, type, description + " " + target);
    }

    public static void PerformRoomAction(GameClient modSession, int roomId, bool kickUsers, bool lockRoom, bool inappropriateRoom)
    {
        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (lockRoom || inappropriateRoom)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();

            if (lockRoom)
            {
                room.RoomData.Access = RoomAccess.Doorbell;
                RoomDao.UpdateState(dbClient, room.Id);
            }

            if (inappropriateRoom)
            {
                room.RoomData.Name = "Inapproprié pour l'hôtel";
                room.RoomData.Description = "Malheureusement, cet appartement ne peut figurer dans le navigateur, car il ne respecte pas nos conditions générales d'utilisations.";
                room.ClearTags();
                room.RoomData.Tags.Clear();

                RoomDao.UpdateCaptionDescTags(dbClient, room.Id);
            }
        }

        if (kickUsers)
        {
            foreach (var roomUser in room.RoomUserManager.GetUserList().ToList())
            {
                if (!roomUser.IsBot && !roomUser.Client.User.HasPermission("no_kick"))
                {
                    room.RoomUserManager.RemoveUserFromRoom(roomUser.Client, true, true);
                }
            }
        }

        room.SendPacket(new GetGuestRoomResultComposer(modSession, room.RoomData, false, false));
    }

    public static void KickUser(GameClient modSession, int userId, string message, bool soft)
    {
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || !clientByUserId.User.InRoom || clientByUserId.User.Id == modSession.User.Id)
        {
            return;
        }

        if (clientByUserId.User.Rank >= modSession.User.Rank)
        {
            modSession.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.kick.missingrank", modSession.Langue));
        }
        else
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(clientByUserId.User.CurrentRoomId, out var room))
            {
                return;
            }

            room.RoomUserManager.RemoveUserFromRoom(clientByUserId, true, false);

            if (soft)
            {
                return;
            }

            if (modSession.User.Antipub(message, "<MT>", room.Id))
            {
                return;
            }

            LogStaffEntry(modSession.User.Id, modSession.User.Username, 0, string.Empty, "Modtool", string.Format("Modtool kickalert: {0}", message));

            clientByUserId.SendNotification(message);
        }
    }

    public static void BanUser(GameClient modSession, int userId, int length, string message)
    {
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.User.Id == modSession.User.Id)
        {
            return;
        }

        if (clientByUserId.User.Rank >= modSession.User.Rank)
        {
            modSession.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.ban.missingrank", modSession.Langue));
        }
        else
        {
            double lengthSeconds = length;
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(clientByUserId, modSession.User.Username, lengthSeconds, message, false, false);
        }
    }
}
