namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chats.Logs;
using WibboEmulator.Utilities;

internal class ModeratorUserChatlogComposer : ServerPacket
{
    public ModeratorUserChatlogComposer(int userId, string username, int roomId, List<ChatlogEntry> chatlogs)
        : base(ServerPacketHeader.MODTOOL_USER_CHATLOG)
    {
        this.WriteInteger(userId);
        this.WriteString(username);
        this.WriteInteger(1);

        this.WriteByte(1);
        this.WriteShort(2);
        this.WriteString("roomName");
        this.WriteByte(2);
        this.WriteString("RoomName"); // room name
        this.WriteString("roomId");
        this.WriteByte(1);
        this.WriteInteger(roomId);

        this.WriteShort(chatlogs.Count);
        foreach (var chat in chatlogs)
        {
            this.WriteString(UnixTimestamp.FromUnixTimestamp(chat.Timestamp).ToShortTimeString());
            this.WriteInteger(chat.UserID);
            this.WriteString(chat.Username);
            this.WriteString(chat.Message);
            this.WriteBoolean(false);
        }
    }
}
