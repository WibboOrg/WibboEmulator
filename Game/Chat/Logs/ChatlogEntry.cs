using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Users;
using System;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Logs
{
    public class ChatlogEntry
    {
        public readonly int userID;
        public readonly string username;
        public readonly int roomID;
        public readonly string message;
        public readonly DateTime timeSpoken;

        public ChatlogEntry(int userID, string username, int roomID, string message, DateTime timeSpoken)
        {
            this.userID = userID;
            this.username = username;
            this.roomID = roomID;
            this.message = message;
            this.timeSpoken = timeSpoken;
        }
    }
}
