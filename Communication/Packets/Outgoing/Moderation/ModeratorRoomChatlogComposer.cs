namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal class ModeratorRoomChatlogComposer : ServerPacket
{
    public ModeratorRoomChatlogComposer(Room room, List<ChatlogEntry> chatlogs)
        : base(ServerPacketHeader.MODTOOL_ROOM_CHATLOG)
    {
        this.WriteByte(1);

        this.WriteShort(2);

        this.WriteString("roomName");
        this.WriteByte(2);
        this.WriteString(room.RoomData.Name);

        this.WriteString("roomId");
        this.WriteByte(1);
        this.WriteInteger(room.RoomData.Id);

        this.WriteShort(chatlogs.Count);
        foreach (var chat in chatlogs)
        {
            if (chat != null)
            {
                this.WriteString(UnixTimestamp.FromUnixTimestamp(chat.timestamp).ToShortTimeString()); //this.timeSpoken.Minute
                this.WriteInteger(chat.userID); //this.timeSpoken.Minute
                this.WriteString(chat.username);
                this.WriteString(chat.message);
                this.WriteBoolean(false); // Text is bold
            }
            else
            {
                this.WriteString("0"); //this.timeSpoken.Minute
                this.WriteInteger(0); //this.timeSpoken.Minute
                this.WriteString("");
                this.WriteString("");
                this.WriteBoolean(false); // Text is bold
            }
        }
    }
}
