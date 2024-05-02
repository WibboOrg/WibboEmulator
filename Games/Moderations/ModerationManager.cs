namespace WibboEmulator.Games.Moderations;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Database.Daos.Moderation;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public static class ModerationManager
{
    public static List<string> UserMessagePresets { get; } = [];
    public static List<ModerationTicket> Tickets { get; } = [];
    public static List<string> RoomMessagePresets { get; } = [];

    private static readonly List<ModerationPresetActionMessages> TicketResolution1 = [];
    private static readonly List<ModerationPresetActionMessages> TicketResolution2 = [];

    private static readonly Dictionary<int, string> ModerationCFHTopics = [];

    private static readonly Dictionary<int, List<ModerationPresetActions>> ModerationCFHTopicActions = [];

    public static void Initialize(IDbConnection dbClient)
    {
        LoadMessageTopics(dbClient);
        LoadMessagePresets(dbClient);
        LoadPendingTickets(dbClient);
        LoadTicketResolution(dbClient);
    }

    public static Dictionary<string, List<ModerationPresetActions>> UserActionPresets
    {
        get
        {
            var result = new Dictionary<string, List<ModerationPresetActions>>();
            foreach (var category in ModerationCFHTopics.ToList())
            {
                result.Add(category.Value, []);

                if (ModerationCFHTopicActions.TryGetValue(category.Key, out var value))
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

    public static void LoadMessageTopics(IDbConnection dbClient)
    {
        var topicList = ModerationTopicDao.GetAll(dbClient);

        if (topicList.Count != 0)
        {
            foreach (var topic in topicList)
            {
                if (!ModerationCFHTopics.ContainsKey(topic.Id))
                {
                    ModerationCFHTopics.Add(topic.Id, topic.Caption);
                }
            }
        }

        var topicActionList = ModerationTopicActionDao.GetAll(dbClient);

        if (topicActionList.Count != 0)
        {
            foreach (var topicAction in topicActionList)
            {
                if (!ModerationCFHTopicActions.ContainsKey(topicAction.ParentId))
                {
                    ModerationCFHTopicActions.Add(topicAction.ParentId, []);
                }

                ModerationCFHTopicActions[topicAction.ParentId].Add(new ModerationPresetActions(topicAction.Id, topicAction.ParentId, topicAction.Type, topicAction.Caption, topicAction.MessageText,
                   topicAction.MuteTime, topicAction.BanTime, topicAction.IpTime, topicAction.TradeLockTime, topicAction.DefaultSanction));
            }
        }
    }

    public static void LoadMessagePresets(IDbConnection dbClient)
    {
        UserMessagePresets.Clear();
        RoomMessagePresets.Clear();

        var presentList = ModerationPresetDao.GetAll(dbClient);

        if (presentList.Count == 0)
        {
            return;
        }

        foreach (var present in presentList)
        {
            switch (present.Type.ToLower())
            {
                case "message":
                    UserMessagePresets.Add(present.Message);
                    continue;
                case "roommessage":
                    RoomMessagePresets.Add(present.Message);
                    continue;
                default:
                    continue;
            }
        }
    }

    public static void LoadTicketResolution(IDbConnection dbClient)
    {
        TicketResolution1.Clear();
        TicketResolution2.Clear();

        var resolutionList = ModerationResolutionDao.GetAll(dbClient);
        if (resolutionList.Count == 0)
        {
            return;
        }

        foreach (var resolution in resolutionList)
        {
            var str = new ModerationPresetActionMessages(resolution.Title, resolution.Subtitle, resolution.BanHours, resolution.EnableMute, resolution.MuteHours, resolution.Reminder, resolution.Message);
            switch (resolution.Type)
            {
                case "Sexual":
                    TicketResolution1.Add(str);
                    continue;
                case "PII":
                    TicketResolution2.Add(str);
                    continue;
                default:
                    continue;
            }
        }
    }

    public static void LoadPendingTickets(IDbConnection dbClient)
    {
        var ticketList = ModerationTicketDao.GetAll(dbClient);

        foreach (var ticket in ticketList)
        {
            var moderationTicket = new ModerationTicket(ticket.Id, ticket.Score, ticket.Type, ticket.SenderId, ticket.ReportedId, ticket.Message, ticket.RoomId, ticket.RoomName, ticket.Timestamp);

            if (ticket.Status.ToLower() == "picked")
            {
                moderationTicket.Pick(ticket.ModeratorId, false);
            }

            Tickets.Add(moderationTicket);
        }
    }

    public static void SendNewTicket(GameClient session, int category, int reportedUser, string message)
    {
        var roomData = RoomManager.GenerateNullableRoomData(session.User.RoomId);
        var id = 0;
        var roomname = (roomData != null) ? roomData.Name : "Aucun appart";
        var roomId = (roomData != null) ? roomData.Id : 0;

        using (var dbClient = DatabaseManager.Connection)
        {
            id = ModerationTicketDao.Insert(dbClient, message, roomname, category, session.User.Id, reportedUser, roomId);
        }

        var ticket = new ModerationTicket(id, 1, category, session.User.Id, reportedUser, message, roomId, roomname, WibboEnvironment.GetUnixTimestamp());
        Tickets.Add(ticket);
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

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
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

    public static ModerationTicket GetTicket(int ticketId)
    {
        foreach (var moderationTicket in Tickets)
        {
            if (moderationTicket.TicketId == ticketId)
            {
                return moderationTicket;
            }
        }
        return null;
    }

    public static void PickTicket(GameClient session, int ticketId)
    {
        var ticket = GetTicket(ticketId);
        if (ticket == null || ticket.Status != TicketStatusType.Open)
        {
            return;
        }

        ticket.Pick(session.User.Id, true);
        SendTicketToModerators(ticket);
    }

    public static void ReleaseTicket(GameClient session, int ticketId)
    {
        var ticket = GetTicket(ticketId);
        if (ticket == null || ticket.Status != TicketStatusType.Picked || ticket.ModeratorId != session.User.Id)
        {
            return;
        }

        ticket.Release(true);
        SendTicketToModerators(ticket);
    }

    public static void CloseTicket(GameClient session, int ticketId, int result)
    {
        var ticket = GetTicket(ticketId);
        if (ticket == null || ticket.Status != TicketStatusType.Picked || ticket.ModeratorId != session.User.Id)
        {
            return;
        }

        var clientByUserId = GameClientManager.GetClientByUserID(ticket.SenderId);

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

    public static bool UsersHasPendingTicket(int id)
    {
        foreach (var moderationTicket in Tickets)
        {
            if (moderationTicket.SenderId == id && moderationTicket.Status == TicketStatusType.Open)
            {
                return true;
            }
        }
        return false;
    }

    public static void DeletePendingTicketForUser(int id)
    {
        foreach (var ticket in Tickets)
        {
            if (ticket.SenderId == id)
            {
                ticket.Delete(true);
                SendTicketToModerators(ticket);
                break;
            }
        }
    }

    public static void SendTicketToModerators(ModerationTicket ticket) => GameClientManager.SendMessageStaff(new ModeratorSupportTicketComposer(ticket));

    public static void LogStaffEntry(int userId, string modName, int roomId, string target, string type, string description)
    {
        using var dbClient = DatabaseManager.Connection;
        LogCommandDao.Insert(dbClient, userId, modName, roomId, target, type, description + " " + target);
    }

    public static void PerformRoomAction(GameClient modSession, int roomId, bool kickUsers, bool lockRoom, bool inappropriateRoom)
    {
        if (!RoomManager.TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (lockRoom || inappropriateRoom)
        {
            using var dbClient = DatabaseManager.Connection;

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
            foreach (var roomUser in room.RoomUserManager.UserList.ToList())
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
        var clientByUserId = GameClientManager.GetClientByUserID(userId);
        if (clientByUserId == null || !clientByUserId.User.InRoom || clientByUserId.User.Id == modSession.User.Id)
        {
            return;
        }

        if (clientByUserId.User.Rank >= modSession.User.Rank)
        {
            modSession.SendNotification(LanguageManager.TryGetValue("moderation.kick.missingrank", modSession.Language));
        }
        else
        {
            if (!RoomManager.TryGetRoom(clientByUserId.User.RoomId, out var room))
            {
                return;
            }

            room.RoomUserManager.RemoveUserFromRoom(clientByUserId, true, false);

            if (soft)
            {
                return;
            }

            if (modSession.User.CheckChatMessage(message, "<MT>", room.Id))
            {
                return;
            }

            LogStaffEntry(modSession.User.Id, modSession.User.Username, 0, string.Empty, "Modtool", string.Format("Modtool kickalert: {0}", message));

            clientByUserId.SendNotification(message);
        }
    }

    public static void BanUser(GameClient modSession, int userId, int lengthSeconds, string message)
    {
        var clientByUserId = GameClientManager.GetClientByUserID(userId);
        if (clientByUserId == null || clientByUserId.User.Id == modSession.User.Id)
        {
            return;
        }

        if (clientByUserId.User.Rank >= modSession.User.Rank)
        {
            modSession.SendNotification(LanguageManager.TryGetValue("moderation.ban.missingrank", modSession.Language));
        }
        else
        {
            GameClientManager.BanUser(clientByUserId, modSession.User.Username, lengthSeconds, message, false);
        }
    }
}
