using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.Users;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Logs
{
    public sealed class ChatlogEntry
    {
        public int _userID;
        public string _username;
        public int _roomID;
        public string _message;
        public DateTime _timeSpoken;

        private WeakReference _playerReference;
        private WeakReference _roomReference;
        public ChatlogEntry(int userID, string username, int roomID, string message, DateTime timeSpoken)
        {
            _userID = userID;
            _username = username;
            _roomID = roomID;
            _message = message;
            _timeSpoken = timeSpoken;
        }

        public void Serialize(ref ServerPacket packet)
        {
            packet.WriteString(_timeSpoken.Hour + ":" + _timeSpoken.Minute); //this.timeSpoken.Minute
            packet.WriteInteger(_userID); //this.timeSpoken.Minute
            packet.WriteString(_username);
            packet.WriteString(_message);
            packet.WriteBoolean(false); // Text is bold
        }

        public int UserID
        {
            get { return _userID; }
        }

        public string UserName
        {
            get { return _username; }
        }

        public string Message
        {
            get { return _message; }
        }

        public int RoomID
        {
            get { return _roomID; }
        }

        public DateTime TimeSpoken
        {
            get { return _timeSpoken; }
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
    }
}
