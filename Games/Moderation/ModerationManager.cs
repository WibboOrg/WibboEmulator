namespace WibboEmulator.Games.Moderation;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Moderation;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;

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
            var Result = new Dictionary<string, List<ModerationPresetActions>>();
            foreach (var Category in this._moderationCFHTopics.ToList())
            {
                Result.Add(Category.Value, new List<ModerationPresetActions>());

                if (this._moderationCFHTopicActions.ContainsKey(Category.Key))
                {
                    foreach (var Data in this._moderationCFHTopicActions[Category.Key])
                    {
                        Result[Category.Value].Add(Data);
                    }
                }
            }
            return Result;
        }
    }

    public void LoadMessageTopics(IQueryAdapter dbClient)
    {
        var moderationTopics = ModerationTopicDao.GetAll(dbClient);

        if (moderationTopics != null)
        {
            foreach (DataRow Row in moderationTopics.Rows)
            {
                if (!this._moderationCFHTopics.ContainsKey(Convert.ToInt32(Row["id"])))
                {
                    this._moderationCFHTopics.Add(Convert.ToInt32(Row["id"]), Convert.ToString(Row["caption"]));
                }
            }
        }

        var ModerationTopicsActions = ModerationTopicActionDao.GetAll(dbClient);

        if (ModerationTopicsActions != null)
        {
            foreach (DataRow Row in ModerationTopicsActions.Rows)
            {
                var ParentId = Convert.ToInt32(Row["parent_id"]);

                if (!this._moderationCFHTopicActions.ContainsKey(ParentId))
                {
                    this._moderationCFHTopicActions.Add(ParentId, new List<ModerationPresetActions>());
                }

                this._moderationCFHTopicActions[ParentId].Add(new ModerationPresetActions(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["parent_id"]), Convert.ToString(Row["type"]), Convert.ToString(Row["caption"]), Convert.ToString(Row["message_text"]),
                    Convert.ToInt32(Row["mute_time"]), Convert.ToInt32(Row["ban_time"]), Convert.ToInt32(Row["ip_time"]), Convert.ToInt32(Row["trade_lock_time"]), Convert.ToString(Row["default_sanction"])));
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
            var ModerationTicket = new ModerationTicket(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["score"]), Convert.ToInt32(dataRow["type"]), Convert.ToInt32(dataRow["sender_id"]), Convert.ToInt32(dataRow["reported_id"]), (string)dataRow["message"], Convert.ToInt32(dataRow["room_id"]), (string)dataRow["room_name"], (double)dataRow["timestamp"]);

            var status = dataRow["status"].ToString();
            if (status?.ToLower() == "picked")
            {
                ModerationTicket.Pick(Convert.ToInt32(dataRow["moderator_id"]), false);
            }

            this._tickets.Add(ModerationTicket);
        }
    }

    public void SendNewTicket(GameClient session, int Category, int ReportedUser, string Message)
    {
        var roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(session.GetUser().CurrentRoomId);
        var Id = 0;
        var roomname = (roomData != null) ? roomData.Name : "Aucun appart";
        var roomId = (roomData != null) ? roomData.Id : 0;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            Id = ModerationTicketDao.Insert(dbClient, Message, roomname, Category, session.GetUser().Id, ReportedUser, roomId);
        }

        var Ticket = new ModerationTicket(Id, 1, Category, session.GetUser().Id, ReportedUser, Message, roomId, roomname, WibboEnvironment.GetUnixTimestamp());
        this._tickets.Add(Ticket);
        SendTicketToModerators(Ticket);
    }

    public void ApplySanction(GameClient session, int reportedUser)
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

        session.GetUser().GetMessenger().DestroyFriendship(userReport.Id);

        session.SendPacket(new IgnoreStatusComposer(1, userReport.Username));

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if ((room.RoomData.BanFuse != 1 || !room.CheckRights(session)) && !room.CheckRights(session, true))
        {
            return;
        }

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(userReport.Id);
        if (roomUserByUserId == null || roomUserByUserId.IsBot || room.CheckRights(roomUserByUserId.GetClient(), true) || roomUserByUserId.GetClient().GetUser().HasPermission("perm_mod") || roomUserByUserId.GetClient().GetUser().HasPermission("perm_no_kick"))
        {
            return;
        }

        room.AddBan(userReport.Id, 429496729);
        room.GetRoomUserManager().RemoveUserFromRoom(roomUserByUserId.GetClient(), true, true);
    }

    public ModerationTicket GetTicket(int TicketId)
    {
        foreach (var ModerationTicket in this._tickets)
        {
            if (ModerationTicket.TicketId == TicketId)
            {
                return ModerationTicket;
            }
        }
        return null;
    }

    public List<string> UserMessagePresets() => this._userMessagePresets;

    public List<ModerationTicket> Tickets() => this._tickets;

    public List<string> RoomMessagePresets() => this._roomMessagePresets;

    public void PickTicket(GameClient session, int TicketId)
    {
        var ticket = this.GetTicket(TicketId);
        if (ticket == null || ticket.Status != TicketStatusType.OPEN)
        {
            return;
        }

        ticket.Pick(session.GetUser().Id, true);
        SendTicketToModerators(ticket);
    }

    public void ReleaseTicket(GameClient session, int TicketId)
    {
        var ticket = this.GetTicket(TicketId);
        if (ticket == null || ticket.Status != TicketStatusType.PICKED || ticket.ModeratorId != session.GetUser().Id)
        {
            return;
        }

        ticket.Release(true);
        SendTicketToModerators(ticket);
    }

    public void CloseTicket(GameClient session, int TicketId, int Result)
    {
        var ticket = this.GetTicket(TicketId);
        if (ticket == null || ticket.Status != TicketStatusType.PICKED || ticket.ModeratorId != session.GetUser().Id)
        {
            return;
        }

        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(ticket.SenderId);

        TicketStatusType NewStatus;
        string MessageAlert;
        switch (Result)
        {
            case 1:
                NewStatus = TicketStatusType.INVALID;
                MessageAlert = "Es-tu certain d'avoir bien utilisé cet outil ? Nous voulons donner le meilleur des services mais nous devons aussi aider d'autres personnes dans l'urgence...";
                break;
            case 2:
                NewStatus = TicketStatusType.ABUSIVE;
                MessageAlert = "Merci de ne pas utiliser l'outil d'appel à l'aide pour rien. Tu risques l'exclusion.";
                break;
            default:
                NewStatus = TicketStatusType.RESOLVED;
                MessageAlert = "Merci, ton souci est résolu ou en cours de résolution. N'hésite pas à Ignorer la personne  ou à la supprimer de ta console s'il s'agit d'insultes.";
                break;
        }
        if (clientByUserId != null)
        {
            clientByUserId.SendPacket(new ModeratorSupportTicketResponseComposer(MessageAlert));
        }
        ticket.Close(NewStatus, true);
        SendTicketToModerators(ticket);
    }

    public bool UsersHasPendingTicket(int Id)
    {
        foreach (var ModerationTicket in this._tickets)
        {
            if (ModerationTicket.SenderId == Id && ModerationTicket.Status == TicketStatusType.OPEN)
            {
                return true;
            }
        }
        return false;
    }

    public void DeletePendingTicketForUser(int Id)
    {
        foreach (var Ticket in this._tickets)
        {
            if (Ticket.SenderId == Id)
            {
                Ticket.Delete(true);
                SendTicketToModerators(Ticket);
                break;
            }
        }
    }

    public void SendTicketToModerators(ModerationTicket Ticket) => WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(new ModeratorSupportTicketComposer(Ticket));

    public void LogStaffEntry(int userId, string modName, int roomId, string target, string type, string description)
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

        if (lockRoom)
        {
            room.RoomData.State = 1;
            room.RoomData.Name = "Cet appart ne respecte par les conditions d'utilisation";
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            RoomDao.UpdateState(dbClient, room.Id);
        }

        if (inappropriateRoom)
        {
            room.RoomData.Name = "Inapproprié pour l'hôtel";
            room.RoomData.Description = "Malheureusement, cet appartement ne peut figurer dans le navigateur, car il ne respecte pas notre Wibbo Attitude ainsi que nos conditions générales d'utilisations.";
            room.ClearTags();
            room.RoomData.Tags.Clear();
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            RoomDao.UpdateCaptionDescTags(dbClient, room.Id);
        }

        if (kickUsers)
        {
            room.OnRoomKick();
        }

        room.SendPacket(new GetGuestRoomResultComposer(modSession, room.RoomData, false, false));
    }

    public void KickUser(GameClient modSession, int userId, string message, bool soft)
    {
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.GetUser().CurrentRoomId < 1 || clientByUserId.GetUser().Id == modSession.GetUser().Id)
        {
            return;
        }

        if (clientByUserId.GetUser().Rank >= modSession.GetUser().Rank)
        {
            modSession.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.kick.missingrank", modSession.Langue));
        }
        else
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(clientByUserId.GetUser().CurrentRoomId, out var room))
            {
                return;
            }

            room.GetRoomUserManager().RemoveUserFromRoom(clientByUserId, true, false);

            if (soft)
            {
                return;
            }

            if (modSession.Antipub(message, "<MT>"))
            {
                return;
            }

            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(modSession.GetUser().Id, modSession.GetUser().Username, 0, string.Empty, "Modtool", string.Format("Modtool kickalert: {0}", message));

            clientByUserId.SendNotification(message);
        }
    }

    public static void BanUser(GameClient ModSession, int UserId, int Length, string Message)
    {
        var clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(UserId);
        if (clientByUserId == null || clientByUserId.GetUser().Id == ModSession.GetUser().Id)
        {
            return;
        }

        if (clientByUserId.GetUser().Rank >= ModSession.GetUser().Rank)
        {
            ModSession.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.ban.missingrank", ModSession.Langue));
        }
        else
        {
            double LengthSeconds = Length;
            WibboEnvironment.GetGame().GetGameClientManager().BanUser(clientByUserId, ModSession.GetUser().Username, LengthSeconds, Message, false, false);
        }
    }
}
