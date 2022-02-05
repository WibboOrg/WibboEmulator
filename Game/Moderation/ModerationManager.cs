using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Moderation;

namespace Butterfly.Game.Moderation
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

        public void Init()
        {
            this.LoadMessageTopics();
            this.LoadMessagePresets();
            this.LoadPendingTickets();
            this.LoadTicketResolution();
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

        public void LoadMessageTopics()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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

                DataTable ModerationTopicsActions = null;
                ModerationTopicsActions = ModerationTopicActionDao.GetAll(dbClient);

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
        }

        public void LoadMessagePresets()
        {
            this._userMessagePresets.Clear();
            this._roomMessagePresets.Clear();
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
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
        }

        public void LoadTicketResolution()
        {
            this._ticketResolution1.Clear();
            this._ticketResolution2.Clear();
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
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
        }

        public void LoadPendingTickets()
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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
        }

        public void SendNewTicket(Client Session, int Category, int ReportedUser, string Message)
        {
            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Session.GetHabbo().CurrentRoomId);
            int Id = 0;
            string roomname = (roomData == null) ? roomData.Name : "Aucun appart";
            int roomid = (roomData == null) ? roomData.Id : 0;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Id = ModerationTicketDao.Insert(dbClient, Message, roomname, Category, Session.GetHabbo().Id, ReportedUser, roomData.Id);
            }

            ModerationTicket Ticket = new ModerationTicket(Id, 1, Category, Session.GetHabbo().Id, ReportedUser, Message, roomid, roomname, ButterflyEnvironment.GetUnixTimestamp());
            this._tickets.Add(Ticket);
            SendTicketToModerators(Ticket);
        }

        public void ApplySanction(Client Session, int ReportedUser)
        {
            if (ReportedUser == 0)
            {
                return;
            }

            User UserReport = ButterflyEnvironment.GetHabboById(ReportedUser);
            if (UserReport == null)
            {
                return;
            }

            Session.GetHabbo().GetMessenger().DestroyFriendship(UserReport.Id);

            Session.SendPacket(new IgnoreStatusComposer(1, UserReport.Username));

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || (room.RoomData.BanFuse != 1 || !room.CheckRights(Session)) && !room.CheckRights(Session, true))
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(UserReport.Id);
            if (roomUserByHabbo == null || roomUserByHabbo.IsBot || (room.CheckRights(roomUserByHabbo.GetClient(), true) || roomUserByHabbo.GetClient().GetHabbo().HasFuse("fuse_mod") || roomUserByHabbo.GetClient().GetHabbo().HasFuse("fuse_no_kick")))
            {
                return;
            }

            room.AddBan(UserReport.Id, 429496729);
            room.GetRoomUserManager().RemoveUserFromRoom(roomUserByHabbo.GetClient(), true, true);
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

        public void PickTicket(Client Session, int TicketId)
        {
            ModerationTicket ticket = this.GetTicket(TicketId);
            if (ticket == null || ticket.Status != TicketStatusType.OPEN)
            {
                return;
            }

            ticket.Pick(Session.GetHabbo().Id, true);
            SendTicketToModerators(ticket);
        }

        public void ReleaseTicket(Client Session, int TicketId)
        {
            ModerationTicket ticket = this.GetTicket(TicketId);
            if (ticket == null || ticket.Status != TicketStatusType.PICKED || ticket.ModeratorId != Session.GetHabbo().Id)
            {
                return;
            }

            ticket.Release(true);
            SendTicketToModerators(ticket);
        }

        public void CloseTicket(Client Session, int TicketId, int Result)
        {
            ModerationTicket ticket = this.GetTicket(TicketId);
            if (ticket == null || ticket.Status != TicketStatusType.PICKED || ticket.ModeratorId != Session.GetHabbo().Id)
            {
                return;
            }

            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(ticket.SenderId);

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
            ButterflyEnvironment.GetGame().GetClientManager().SendMessageStaff(new ModeratorSupportTicketComposer(Ticket));
        }

        public void LogStaffEntry(int userId, string modName, int roomId, string target, string type, string description)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                LogCommandDao.Insert(dbClient, userId, modName, roomId, target, type, description + " " + target);
            }
        }

        public static void PerformRoomAction(Client ModSession, int RoomId, bool KickUsers, bool LockRoom, bool InappropriateRoom)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null)
            {
                return;
            }

            if (LockRoom)
            {
                room.RoomData.State = 1;
                room.RoomData.Name = "Cet appart ne respect par les conditions d'utilisation";
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    RoomDao.UpdateState(dbClient, room.Id);
                }
            }

            if (InappropriateRoom)
            {
                room.RoomData.Name = "Inapproprié pour l'hôtel";
                room.RoomData.Description = "Malheureusement, cet appartement ne peut pas figurer dans le navigateur, car il ne respecte pas notre Wibbo Attitude ainsi que nos conditions générales d'utilisations.";
                room.ClearTags();
                room.RoomData.Tags.Clear();
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    RoomDao.UpdateCaptionDescTags(dbClient, room.Id);
                }
            }

            if (KickUsers)
            {
                room.onRoomKick();
            }

            room.SendPacket(new GetGuestRoomResultComposer(ModSession, room.RoomData, false, false));
        }

        public static void KickUser(Client ModSession, int UserId, string Message, bool Soft)
        {
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (clientByUserId == null || clientByUserId.GetHabbo().CurrentRoomId < 1 || clientByUserId.GetHabbo().Id == ModSession.GetHabbo().Id)
            {
                return;
            }

            if (clientByUserId.GetHabbo().Rank >= ModSession.GetHabbo().Rank)
            {
                ModSession.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.kick.missingrank", ModSession.Langue));
            }
            else
            {
                Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetHabbo().CurrentRoomId);
                if (room == null)
                {
                    return;
                }

                room.GetRoomUserManager().RemoveUserFromRoom(clientByUserId, true, false);

                if (Soft)
                {
                    return;
                }

                if (ModSession.Antipub(Message, "<MT>"))
                {
                    return;
                }

                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(ModSession.GetHabbo().Id, ModSession.GetHabbo().Username, 0, string.Empty, "Modtool", string.Format("Modtool kickalert: {0}", Message));

                clientByUserId.SendNotification(Message);
            }
        }

        public static void BanUser(Client ModSession, int UserId, int Length, string Message)
        {
            Client clientByUserId = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (clientByUserId == null || clientByUserId.GetHabbo().Id == ModSession.GetHabbo().Id)
            {
                return;
            }

            if (clientByUserId.GetHabbo().Rank >= ModSession.GetHabbo().Rank)
            {
                ModSession.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.ban.missingrank", ModSession.Langue));
            }
            else
            {
                double LengthSeconds = Length;
                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUserId, ModSession.GetHabbo().Username, LengthSeconds, Message, false, false);
            }
        }
    }
}
