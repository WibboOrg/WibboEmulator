using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.Chat.Logs;
using Butterfly.HabboHotel.Users;
using Butterfly.Utilities;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Moderation
{
    internal class ModeratorUserChatlogMessageComposer : ServerPacket
    {
        public ModeratorUserChatlogMessageComposer(Habbo habbo, List<KeyValuePair<RoomData, List<ChatlogEntry>>> chatlogs)
            : base(ServerPacketHeader.MODTOOL_USER_CHATLOG)
        {
            WriteInteger(habbo.Id);
            WriteString(habbo.Username);

            WriteInteger(chatlogs.Count);
            
            foreach (KeyValuePair<RoomData, List<ChatlogEntry>> Chatlog in chatlogs)
            {
                WriteByte(1);
                WriteShort(2);//Count
                WriteString("roomName");
                WriteByte(2);
                WriteString(Chatlog.Key.Name); // room name
                WriteString("roomId");
                WriteByte(1);
                WriteInteger(Chatlog.Key.Id);

                WriteShort(Chatlog.Value.Count);
                foreach (ChatlogEntry Entry in Chatlog.Value)
                {
                    string Username = "INCONNUE";
                    if (Entry.PlayerNullable() != null)
                    {
                        Username = Entry.PlayerNullable().Username;
                    }

                    WriteString(UnixTimestamp.FromUnixTimestamp(Entry.TimeSpoken.Hour).ToShortTimeString());
                    WriteInteger(Entry.UserID);
                    WriteString(Username);
                    WriteString(!string.IsNullOrEmpty(Entry.Message) ? Entry.Message : "*flood*");
                    WriteBoolean(habbo.Id == Entry.UserID);
                }
            }
        }
    }
}
