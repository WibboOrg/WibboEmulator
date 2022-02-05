namespace Butterfly.Game.Users.Messenger
{
    public class MessengerRequest
    {
        private readonly int _toUser;
        private readonly int _fromUser;
        private readonly string _username;

        public int To => this._toUser;
        public int From => this._fromUser;
        public string Username => this._username;

        public MessengerRequest(int toUser, int fromUser, string username)
        {
            this._toUser = toUser;
            this._fromUser = fromUser;
            this._username = username;
        }
    }
}
