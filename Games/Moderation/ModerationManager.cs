using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using System.Data;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Action;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;

namespace WibboEmulator.Games.Moderation
{
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
                Dictionary<string, List<ModerationPresetActions>> Result = new Dictionary<string, List<ModerationPresetActions>>();
                foreach (KeyValuePair<int, string> Category in this._moderationCFHTopics.ToList())
                {
                    Result.Add(Category.Value, new List<ModerationPresetActions>());

                    if (this._moderationCFHTopicActions.ContainsKey(Category.Key))
                    {
                        foreach (ModerationPresetActions Data in this._moderationCFHTopicActions[Category.Key])
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
            DataTable moderationTopics = ModerationTopicDao.GetAll(dbClient);

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

            DataTable ModerationTopicsActions = ModerationTopicActionDao.GetAll(dbClient);

            if (ModerationTopicsActions != null)
            {
                foreach (DataRow Row in ModerationTopicsActions.Rows)
                {
                    int ParentId = Convert.ToInt32(Row["parent_id"]);

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

            DataTable table = ModerationPresetDao.GetAll(dbClient);
            foreach (DataRow dataRow in table.Rows)
            {
                string str = (string)dataRow["message"];
                switch (dataRow["type"].ToString().ToLower())
                {
                    case "message":
                        this._userMessagePresets.Add(str);
                        continue;
                    case "roommessage":
                        this._roomMessagePresets.Add(str);
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

            DataTable table = ModerationResolutionDao.GetAll(dbClient);
            foreach (DataRow dataRow in table.Rows)
            {
                ModerationPresetActionMessages str = new ModerationPresetActionMessages((string)dataRow["title"], (string)dataRow["subtitle"], Convert.ToInt32(dataRow["ban_hours"]), Convert.ToInt32(dataRow["enable_mute"]), Convert.ToInt32(dataRow["mute_hours"]), Convert.ToInt32(dataRow["reminder"]), (string)dataRow["message"]);
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
            DataTable table = ModerationTicketDao.GetAll(dbClient);
            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                ModerationTicket ModerationTicket = new ModerationTicket(Convert.ToInt32(dataRow["id"]), Convert.ToInt32(dataRow["score"]), Convert.ToInt32(dataRow["type"]), Convert.ToInt32(dataRow["sender_id"]), Convert.ToInt32(dataRow["reported_id"]), (string)dataRow["message"], Convert.ToInt32(dataRow["room_id"]), (string)dataRow["room_name"], (double)dataRow["timestamp"]);
                if (dataRow["status"].ToString().ToLower() == "picked")
                {
                    ModerationTicket.Pick(Convert.ToInt32(dataRow["moderator_id"]), false);
                }

                this._tickets.Add(ModerationTicket);
            }
        }

        public void SendNewTicket(GameClient Session, int Category, int ReportedUser, string Message)
        {
            RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Session.GetUser().CurrentRoomId);
            int Id = 0;
            string roomname = (roomData == null) ? roomData.Name : "Aucun appart";
            int roomid = (roomData == null) ? roomData.Id : 0;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Id = ModerationTicketDao.Insert(dbClient, Message, roomname, Category, Session.GetUser().Id, ReportedUser, roomData.Id);
            }

            ModerationTicket Ticket = new ModerationTicket(Id, 1, Category, Session.GetUser().Id, ReportedUser, Message, roomid, roomname, WibboEnvironment.GetUnixTimestamp());
            this._tickets.Add(Ticket);
            SendTicketToModerators(Ticket);
        }

        public void ApplySanction(GameClient Session, int ReportedUser)
        {
            if (ReportedUser == 0)
            {
                return;
            }

            User UserReport = WibboEnvironment.GetUserById(ReportedUser);
            if (UserReport == null)
            {
                return;
            }

            Session.GetUser().GetMessenger().DestroyFriendship(UserReport.Id);

            Session.SendPacket(new IgnoreStatusComposer(1, UserReport.Username));

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if ((room.RoomData.BanFuse != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true))
            {
                return;
            }

            RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(UserReport.Id);
            if (roomUserByUserId == null || roomUserByUserId.IsBot || (room.CheckRights(roomUserByUserId.GetClient(), true) || roomUserByUserId.GetClient().GetUser().HasPermission("perm_mod") || roomUserByUserId.GetClient().GetUser().HasPermission("perm_no_kick")))
            {
                return;
            }

            room.AddBan(UserReport.Id, 429496729);
            room.GetRoomUserManager().RemoveUserFromRoom(roomUserByUserId.GetClient(), true, true);
        }

        public ModerationTicket GetTicket(int TicketId)
        {
            foreach (ModerationTicket ModerationTicket in this._tickets)
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

        public void PickTicket(GameClient Session, int TicketId)
        {
            ModerationTicket ticket = this.GetTicket(TicketId);
            if (ticket == null || ticket.Status != TicketStatusType.OPEN)
            {
                return;
            }

            ticket.Pick(Session.GetUser().Id, true);
            SendTicketToModerators(ticket);
        }

        public void ReleaseTicket(GameClient Session, int TicketId)
        {
            ModerationTicket ticket = this.GetTicket(TicketId);
            if (ticket == null || ticket.Status != TicketStatusType.PICKED || ticket.ModeratorId != Session.GetUser().Id)
            {
                return;
            }

            ticket.Release(true);
            SendTicketToModerators(ticket);
        }

        public void CloseTicket(GameClient Session, int TicketId, int Result)
        {
            ModerationTicket ticket = this.GetTicket(TicketId);
            if (ticket == null || ticket.Status != TicketStatusType.PICKED || ticket.ModeratorId != Session.GetUser().Id)
            {
                return;
            }

            GameClient clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(ticket.SenderId);

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
            foreach (ModerationTicket ModerationTicket in this._tickets)
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
            foreach (ModerationTicket Ticket in this._tickets)
            {
                if (Ticket.SenderId == Id)
                {
                    Ticket.Delete(true);
                    SendTicketToModerators(Ticket);
                    break;
                }
            }
        }

        public static void SendTicketToModerators(ModerationTicket Ticket)
        {
            WibboEnvironment.GetGame().GetGameClientManager().SendMessageStaff(new ModeratorSupportTicketComposer(Ticket));
        }

        public void LogStaffEntry(int userId, string modName, int roomId, string target, string type, string description)
        {
            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            LogCommandDao.Insert(dbClient, userId, modName, roomId, target, type, description + " " + target);
        }

        public static void PerformRoomAction(GameClient modSession, int roomId, bool kickUsers, bool lockRoom, bool inappropriateRoom)
        {
            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room room))
                return;

            if (lockRoom)
            {
                room.RoomData.State = 1;
                room.RoomData.Name = "Cet appart ne respecte par les conditions d'utilisation";
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                RoomDao.UpdateState(dbClient, room.Id);
            }

            if (inappropriateRoom)
            {
                room.RoomData.Name = "Inapproprié pour l'hôtel";
                room.RoomData.Description = "Malheureusement, cet appartement ne peut figurer dans le navigateur, car il ne respecte pas notre Wibbo Attitude ainsi que nos conditions générales d'utilisations.";
                room.ClearTags();
                room.RoomData.Tags.Clear();
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                RoomDao.UpdateCaptionDescTags(dbClient, room.Id);
            }

            if (kickUsers)
            {
                room.OnRoomKick();
            }

            room.SendPacket(new GetGuestRoomResultComposer(modSession, room.RoomData, false, false));
        }

        public static void KickUser(GameClient ModSession, int UserId, string Message, bool Soft)
        {
            GameClient clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(UserId);
            if (clientByUserId == null || clientByUserId.GetUser().CurrentRoomId < 1 || clientByUserId.GetUser().Id == ModSession.GetUser().Id)
            {
                return;
            }

            if (clientByUserId.GetUser().Rank >= ModSession.GetUser().Rank)
            {
                ModSession.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.kick.missingrank", ModSession.Langue));
            }
            else
            {
                if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(clientByUserId.GetUser().CurrentRoomId, out Room room))
                    return;

                room.GetRoomUserManager().RemoveUserFromRoom(clientByUserId, true, false);

                if (Soft)
                {
                    return;
                }

                if (ModSession.Antipub(Message, "<MT>"))
                {
                    return;
                }

                WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(ModSession.GetUser().Id, ModSession.GetUser().Username, 0, string.Empty, "Modtool", string.Format("Modtool kickalert: {0}", Message));

                clientByUserId.SendNotification(Message);
            }
        }

        public static void BanUser(GameClient ModSession, int UserId, int Length, string Message)
        {
            GameClient clientByUserId = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(UserId);
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
}
