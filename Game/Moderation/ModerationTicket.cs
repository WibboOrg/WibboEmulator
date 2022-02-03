using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;

namespace Butterfly.Game.Moderation
{
    public class ModerationTicket
    {
        public int Id;
        public int Score;
        public int Type;
        public TicketStatusType Status;
        public int SenderId;
        public int ReportedId;
        public int ModeratorId;
        public string Message;
        public int RoomId;
        public string RoomName;
        public double Timestamp;
        public string SenderName;
        public string ReportedName;
        public string ModName;

        public int TabId
        {
            get
            {
                if (this.Status == TicketStatusType.OPEN)
                {
                    return 1;
                }

                if (this.Status == TicketStatusType.PICKED || this.Status == TicketStatusType.ABUSIVE || (this.Status == TicketStatusType.INVALID || this.Status == TicketStatusType.RESOLVED))
                {
                    return 2;
                }

                return this.Status == TicketStatusType.DELETED ? 3 : 0;
            }
        }

        public int TicketId => this.Id;

        public ModerationTicket(int Id, int Score, int Type, int SenderId, int ReportedId, string Message, int RoomId, string RoomName, double Timestamp)
        {
            this.Id = Id;
            this.Score = Score;
            this.Type = Type;
            this.Status = TicketStatusType.OPEN;
            this.SenderId = SenderId;
            this.ReportedId = ReportedId;
            this.ModeratorId = 0;
            this.Message = Message;
            this.RoomId = RoomId;
            this.RoomName = RoomName;
            this.Timestamp = Timestamp;
            this.SenderName = this.GetNameById(SenderId);
            this.ReportedName = this.GetNameById(ReportedId);
            this.ModName = this.GetNameById(this.ModeratorId);
        }

        public string GetNameById(int Id)
        {
            string username = "";
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                username = UserDao.GetNameById(dbClient, Id);
            }

            return username;
        }

        public ServerPacket Serialize(ServerPacket message)
        {
            message.WriteInteger(this.Id); // id
            message.WriteInteger(this.TabId); // state
            message.WriteInteger(4); // type (3 or 4 for new style)
            message.WriteInteger(this.Type); // priority
            message.WriteInteger((int)(ButterflyEnvironment.GetUnixTimestamp() - this.Timestamp) * 1000); // -->> timestamp
            message.WriteInteger(this.Score); // priority
            message.WriteInteger(this.SenderId);
            message.WriteInteger(this.SenderId); // sender id 8 ints
            message.WriteString(this.SenderName); // sender name
            message.WriteInteger(this.ReportedId);
            message.WriteString(this.ReportedName);
            message.WriteInteger((this.Status == TicketStatusType.PICKED) ? this.ModeratorId : 0); // mod id
            message.WriteString(this.ModName); // mod name
            message.WriteString(this.Message); // issue message
            message.WriteInteger(0);
            message.WriteInteger(0);

            return message;
        }

        public void Pick(int moderatorId, bool UpdateInDb)
        {
            this.Status = TicketStatusType.PICKED;
            this.ModeratorId = moderatorId;
            this.Timestamp = ButterflyEnvironment.GetUnixTimestamp();

            if (!UpdateInDb)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ModerationTicketDao.UpdateStatusPicked(dbClient, moderatorId, this.Id);
            }
        }

        public void Close(TicketStatusType NewStatus, bool UpdateInDb)
        {
            this.Status = NewStatus;
            if (!UpdateInDb)
            {
                return;
            }

            string str;
            switch (NewStatus)
            {
                case TicketStatusType.ABUSIVE:
                    str = "abusive";
                    break;
                case TicketStatusType.INVALID:
                    str = "invalid";
                    break;
                default:
                    str = "resolved";
                    break;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ModerationTicketDao.UpdateStatus(dbClient, str, this.Id);
            }
        }

        public void Release(bool UpdateInDb)
        {
            this.Status = TicketStatusType.OPEN;

            if (!UpdateInDb)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ModerationTicketDao.UpdateStatusOpen(dbClient, this.Id);
            }
        }

        public void Delete(bool UpdateInDb)
        {
            this.Status = TicketStatusType.DELETED;

            if (!UpdateInDb)
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ModerationTicketDao.UpdateStatusDeleted(dbClient, this.Id);
            }
        }

        public ServerPacket Serialize()
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.ISSUE_INFO);
            serverMessage.WriteInteger(this.Id);
            serverMessage.WriteInteger(this.TabId);
            serverMessage.WriteInteger(3);
            serverMessage.WriteInteger(this.Type);
            serverMessage.WriteInteger((int)(ButterflyEnvironment.GetUnixTimestamp() - this.Timestamp) * 1000);
            serverMessage.WriteInteger(this.Score);
            serverMessage.WriteInteger(this.SenderId);
            serverMessage.WriteInteger(this.SenderId);
            if (ButterflyEnvironment.GetHabboById(this.SenderId) != null)
            {
                serverMessage.WriteString(this.SenderName.Equals("") ? ButterflyEnvironment.GetHabboById(this.SenderId).Username : this.SenderName);
            }
            else
            {
                serverMessage.WriteString(this.SenderName);
            }

            serverMessage.WriteInteger(this.ReportedId);
            if (ButterflyEnvironment.GetHabboById(this.ReportedId) != null)
            {
                serverMessage.WriteString(this.ReportedName.Equals("") ? ButterflyEnvironment.GetHabboById(this.ReportedId).Username : this.ReportedName);
            }
            else
            {
                serverMessage.WriteString(this.ReportedName);
            }

            serverMessage.WriteInteger(this.Status == TicketStatusType.PICKED ? this.ModeratorId : 0);
            if (ButterflyEnvironment.GetHabboById(this.ModeratorId) != null)
            {
                serverMessage.WriteString(this.Status == TicketStatusType.PICKED ? (this.ModName.Equals("") ? ButterflyEnvironment.GetHabboById(this.ModeratorId).Username : this.ModName) : "");
            }
            else
            {
                serverMessage.WriteString(this.ModName);
            }

            serverMessage.WriteString(this.Message);
            serverMessage.WriteInteger(0);
            serverMessage.WriteInteger(0);

            return serverMessage;
        }
    }
}
