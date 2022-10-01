using WibboEmulator.Games.Chat.Logs;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserChatlogComposer : ServerPacket
    {
        public ModeratorUserChatlogComposer(int userId, string username, int roomId, List<ChatlogEntry> chatlogs)
            : base(ServerPacketHeader.MODTOOL_USER_CHATLOG)
        {
            WriteInteger(userId);
            WriteString(username);
            WriteInteger(1);

            WriteByte(1);
            WriteShort(2);
            WriteString("roomName");
            WriteByte(2);
            WriteString("RoomName"); // room name
            WriteString("roomId");
            WriteByte(1);
            WriteInteger(roomId);

            WriteShort(chatlogs.Count);
            foreach (ChatlogEntry chat in chatlogs)
            {
                WriteString(UnixTimestamp.FromUnixTimestamp(chat.timestamp).ToShortTimeString());
                WriteInteger(chat.userID);
                WriteString(chat.username);
                WriteString(chat.message);
                WriteBoolean(false);
            }
        }
    }
}
