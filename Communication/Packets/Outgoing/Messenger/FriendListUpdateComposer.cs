using Butterfly.Game.Users.Messenger;

namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class FriendListUpdateComposer : ServerPacket
    {
        public FriendListUpdateComposer(MessengerBuddy friend, int friendId = 0)
            : base(ServerPacketHeader.MESSENGER_UPDATE)
        {
            WriteInteger(0);
            WriteInteger(1);
            WriteInteger(friend != null ? 0 : -1);

            if (friend != null)
            {
                WriteInteger(friend.UserId);
                WriteString(friend.Username);
                WriteInteger(1);
                bool isOnline = friend.IsOnline;
                WriteBoolean(isOnline);

                if (isOnline)
                {
                    WriteBoolean(!friend.HideInRoom);
                }
                else
                {
                    WriteBoolean(false);
                }

                WriteString(isOnline ? friend.Look : "");
                WriteInteger(0);
                WriteString(""); //Motto ?
                WriteString(string.Empty);
                WriteString(string.Empty);
                WriteBoolean(true); // Allows offline messaging
                WriteBoolean(false);
                WriteBoolean(false);
                WriteShort(friend.Relation);
                WriteBoolean(false);
            }
            else
            {
                WriteInteger(friendId);
            }
        }
    }
}
