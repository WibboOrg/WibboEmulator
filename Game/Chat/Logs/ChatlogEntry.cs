namespace Wibbo.Game.Chat.Logs
{
    public class ChatlogEntry
    {
        public readonly int userID;
        public readonly string username;
        public readonly int roomID;
        public readonly string message;
        public readonly double timestamp;

        public ChatlogEntry(int userID, string username, int roomID, string message, double timestamp)
        {
            this.userID = userID;
            this.username = username;
            this.roomID = roomID;
            this.message = message;
            this.timestamp = timestamp;
        }
    }
}
