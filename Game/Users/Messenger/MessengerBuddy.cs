using Butterfly.Game.Clients;

namespace Butterfly.Game.Users.Messenger
{
    public class MessengerBuddy
    {
        private readonly int _userId;
        private readonly string _username;
        private string _look;
        private int _relation;
        private bool _isOnline;
        private bool _hideInRoom;

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
            Client client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this._userId);
            if (client != null && client.GetHabbo() != null && client.GetHabbo().GetMessenger() != null && !client.GetHabbo().GetMessenger().AppearOffline)
            {
                this._isOnline = true;
                this._look = client.GetHabbo().Look;
                this._hideInRoom = client.GetHabbo().HideInRoom;
            }
            else
            {
                this._isOnline = false;
                this._look = "";
                this._hideInRoom = true;
            }
        }

        public int UserId => this._userId;
        public string Username => this._username;
        public string Look => this. _look;
        public int Relation => this._relation;
        public bool IsOnline => this._isOnline;
        public bool HideInRoom => this._hideInRoom;
    }
}
