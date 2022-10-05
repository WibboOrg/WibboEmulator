using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomChatlogComposer : ServerPacket
    {
        public ModeratorRoomChatlogComposer(Room room, List<ChatlogEntry> chatlogs)
            : base(ServerPacketHeader.MODTOOL_ROOM_CHATLOG)
        {
            WriteByte(1);

            WriteShort(2);

            WriteString("roomName");
            WriteByte(2);
            WriteString(room.RoomData.Name);

            WriteString("roomId");
            WriteByte(1);
            WriteInteger(room.RoomData.Id);

            WriteShort(chatlogs.Count);
            foreach (ChatlogEntry chat in chatlogs)
            {
                if (chat != null)
                {
                    WriteString(UnixTimestamp.FromUnixTimestamp(chat.timestamp).ToShortTimeString()); //this.timeSpoken.Minute
                    WriteInteger(chat.userID); //this.timeSpoken.Minute
                    WriteString(chat.username);
                    WriteString(chat.message);
                    WriteBoolean(false); // Text is bold
                }
                else
                {
                    WriteString("0"); //this.timeSpoken.Minute
                    WriteInteger(0); //this.timeSpoken.Minute
                    WriteString("");
                    WriteString("");
                    WriteBoolean(false); // Text is bold
                }
            }
        }
    }
}
