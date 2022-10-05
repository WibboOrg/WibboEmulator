namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.Users.Messenger;

internal class BuddyListComposer : ServerPacket
{
    public BuddyListComposer(Dictionary<int, MessengerBuddy> friends)
        : base(ServerPacketHeader.MESSENGER_FRIENDS)
    {
        this.WriteInteger(1);
        this.WriteInteger(0);
        this.WriteInteger(friends.Count);
        foreach (var friend in friends.Values)
        {
            this.WriteInteger(friend.UserId);
            this.WriteString(friend.Username);
            this.WriteInteger(1);
            var isOnline = friend.IsOnline;
            this.WriteBoolean(isOnline);

            if (isOnline)
            {
                this.WriteBoolean(!friend.HideInRoom);
            }
            else
            {
                this.WriteBoolean(false);
            }

            this.WriteString(isOnline ? friend.Look : "");
            this.WriteInteger(0);
            this.WriteString(""); //Motto ?
            this.WriteString(string.Empty);
            this.WriteString(string.Empty);
            this.WriteBoolean(true); // Allows offline messaging
            this.WriteBoolean(false);
            this.WriteBoolean(false);
            this.WriteShort(friend.Relation);
        }
    }
}
