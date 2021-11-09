using Butterfly.HabboHotel.Rooms.Chat.Logs;
using Butterfly.HabboHotel.Rooms;
using Butterfly.Utilities;
using System.Collections.Generic;


namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorRoomChatlogMessageComposer : ServerPacket
    {
        public ModeratorRoomChatlogMessageComposer(Room room, ICollection<ChatlogEntry> chats)
            : base(ServerPacketHeader.MODTOOL_ROOM_CHATLOG)
        {
            WriteByte(1);
            WriteShort(2);//Count
            WriteString("roomName");
            WriteByte(2);
            WriteString(room.Name);
            WriteString("roomId");
            WriteByte(1);
            WriteInteger(room.Id);
            WriteShort(chats.Count);

            foreach (ChatlogEntry Entry in chats)
            {
                string Username = "Inconnue";
                if (Entry.PlayerNullable() != null)
                {
                    Username = Entry.PlayerNullable().Username;
                }
                WriteString(UnixTimestamp.FromUnixTimestamp(Entry.TimeSpoken.Hour).ToShortTimeString()); //Temps 
                WriteInteger(Entry.UserID);
                WriteString(Username);
                WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "*flood*");
                WriteBoolean(false);
            }
        }
    }
}
