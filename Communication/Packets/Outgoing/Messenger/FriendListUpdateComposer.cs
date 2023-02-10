namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.Users.Messenger;

internal sealed class FriendListUpdateComposer : ServerPacket
{
    public FriendListUpdateComposer(MessengerBuddy friend, int friendId = 0)
        : base(ServerPacketHeader.MESSENGER_UPDATE)
    {
        this.WriteInteger(0);
        this.WriteInteger(1);
        this.WriteInteger(friend != null ? 0 : -1);

        if (friend != null)
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
            this.WriteBoolean(false);
        }
        else
        {
            this.WriteInteger(friendId);
        }
    }
}
