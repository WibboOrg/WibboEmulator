using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Users;
using System;

namespace Butterfly.Game.Rooms.Chat.Logs
{
    public class ChatMessage
    {
        public readonly int userID;
        public readonly string username;
        public readonly int roomID;
        public readonly string message;
        public readonly DateTime timeSpoken;

        private WeakReference _playerReference;
        private WeakReference _roomReference;

        public ChatMessage(int userID, string username, int roomID, string message, DateTime timeSpoken, Habbo Username = null, RoomData Instance = null)
        {
            this.userID = userID;
            this.username = username;
            this.roomID = roomID;
            this.message = message;
            this.timeSpoken = timeSpoken;

            if (username != null)
                _playerReference = new WeakReference(username);

            if (Instance != null)
                _roomReference = new WeakReference(Instance);
        }

        public void Serialize(ref ServerPacket packet)
        {
            packet.WriteString(this.timeSpoken.Hour + ":" + this.timeSpoken.Minute); //this.timeSpoken.Minute
            packet.WriteInteger(this.userID); //this.timeSpoken.Minute
            packet.WriteString(this.username);
            packet.WriteString(this.message);
            packet.WriteBoolean(false); // Text is bold
        }

        public Habbo PlayerNullable()
        {
            if (_playerReference.IsAlive)
            {
                Habbo PlayerObj = (Habbo)_playerReference.Target;

                return PlayerObj;
            }

            return null;
        }

        public Room RoomNullable()
        {
            if (_roomReference.IsAlive)
            {
                Room RoomObj = (Room)_roomReference.Target;
                if (RoomObj.Disposed)
                    return null;
                return RoomObj;
            }
            return null;
        }

        public int UserID
        {
            get { return userID; }
        }

        public string UserName
        {
            get { return username; }
        }

        public int RoomId
        {
            get { return roomID; }
        }

        public string Message
        {
            get { return message; }
        }

        public DateTime TimeSpoken
        {
            get { return TimeSpoken; }
        }
    }
}
