using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Chat.Logs;
using Butterfly.Utilities;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomChatlogMessageComposer : ServerPacket
    {
        public ModeratorRoomChatlogMessageComposer(Room room, ICollection<ChatMessage> chats)
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
            foreach (ChatMessage Entry in chats)
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
