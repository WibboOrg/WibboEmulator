using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Users.Messenger
{
    public class MessengerBuddy
    {
        private readonly int _userId;
        private readonly string _username;
        private string _look;
        private int _relation;
        private bool _isOnline;
        private bool _hideInroom;

        public MessengerBuddy(int UserId, string Username, string Look, int Relation)
        {
            this._userId = UserId;
            this._username = Username;
            this._look = Look;
            this._relation = Relation;
        }

        public void UpdateRelation(int Type)
        {
            this._relation = Type;
        }

        public void UpdateUser()
        {
            GameClient client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userId);
            if (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null && !client.GetHabbo().GetMessenger().AppearOffline)
            {
                this._isOnline = true;
                this._look = client.GetHabbo().Look;
                this._hideInroom = client.GetHabbo().HideInRoom;
            }
            else
            {
                this._isOnline = false;
                this._look = "";
                this._hideInroom = true;
            }

        }

        public void Serialize(ServerPacket reply)
        {
            reply.WriteInteger(this._userId);
            reply.WriteString(this._username);
            reply.WriteInteger(1);
            bool isOnline = this._isOnline;
            reply.WriteBoolean(isOnline);

            if (isOnline)
            {
                reply.WriteBoolean(!this._hideInroom);
            }
            else
            {
                reply.WriteBoolean(false);
            }

            reply.WriteString(isOnline ? this._look : "");
            reply.WriteInteger(0);
            reply.WriteString(""); //Motto ?
            reply.WriteString(string.Empty);
            reply.WriteString(string.Empty);
            reply.WriteBoolean(true); // Allows offline messaging
            reply.WriteBoolean(false);
            reply.WriteBoolean(false);
            reply.WriteShort(this._relation);
        }
    }
}
