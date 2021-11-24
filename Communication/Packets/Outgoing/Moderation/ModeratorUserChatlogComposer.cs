using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.Chat.Logs;
using Butterfly.Game.User;
using Butterfly.Utility;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserChatlogComposer : ServerPacket
    {
        public ModeratorUserChatlogComposer(Habbo habbo, List<KeyValuePair<RoomData, List<ChatMessage>>> chatlogs)
            : base(ServerPacketHeader.MODTOOL_USER_CHATLOG)
        {
            WriteInteger(habbo.Id);
            WriteString(habbo.Username);

            WriteInteger(chatlogs.Count); // Room Visits Count
            foreach (KeyValuePair<RoomData, List<ChatMessage>> Chatlog in chatlogs)
            {
                WriteByte(1);
                WriteShort(2);//Count
                WriteString("roomName");
                WriteByte(2);
                WriteString(Chatlog.Key.Name); // room name
                WriteString("roomId");
                WriteByte(1);
                WriteInteger(Chatlog.Key.Id);

                WriteShort(Chatlog.Value.Count); // Chatlogs Count
                foreach (ChatMessage Entry in Chatlog.Value)
                {
                    string Username = "NOT FOUND";
                    if (Entry.PlayerNullable() != null)
                    {
                        Username = Entry.PlayerNullable().Username;
                    }

                    WriteString(UnixTimestamp.FromUnixTimestamp(Entry.TimeSpoken.Hour).ToShortTimeString());
                    WriteInteger(Entry.UserID); // UserId of message
                    WriteString(Username); // Username of message
                    WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "** flood **"); // Message        
                    WriteBoolean(habbo.Id == Entry.UserID);
                }
            }
        }
    }
}
