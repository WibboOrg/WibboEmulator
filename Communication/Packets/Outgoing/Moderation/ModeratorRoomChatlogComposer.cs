using Butterfly.Game.Rooms;
using Butterfly.Game.Chat.Logs;
using Butterfly.Utility;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomChatlogComposer : ServerPacket
    {
        public ModeratorRoomChatlogComposer(Room room, ICollection<ChatlogEntry> chats)
            : base(ServerPacketHeader.MODTOOL_ROOM_CHATLOG)
        {
            WriteByte(1);
            WriteShort(2);
            WriteString("roomName");
            WriteByte(2);
            WriteString(room.RoomData.Name);
            WriteString("roomId");
            WriteByte(1);
            WriteInteger(room.Id);

            WriteShort(chats.Count);
            foreach (ChatlogEntry Entry in chats)
            {
                string Username = "Inconnu";
                if (Entry.PlayerNullable() != null)
                {
                    Username = Entry.PlayerNullable().Username;
                }

                WriteString(UnixTimestamp.FromUnixTimestamp(Entry.TimeSpoken.Hour).ToShortTimeString()); // time?
                WriteInteger(Entry.UserID); // User Id
                WriteString(Username); // Username
                WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** flood **"); // Message        
                WriteBoolean(false); //TODO, AI's?
            }
        }
    }
}
