using Butterfly.Game.Users.Messenger;

namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class BuddyListComposer : ServerPacket
    {
        public BuddyListComposer(Dictionary<int, MessengerBuddy> friends)
            : base(ServerPacketHeader.MESSENGER_FRIENDS)
        {
            WriteInteger(1);
            WriteInteger(0);
            WriteInteger(friends.Count);
            foreach (MessengerBuddy friend in friends.Values)
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
            }
        }
    }
}
