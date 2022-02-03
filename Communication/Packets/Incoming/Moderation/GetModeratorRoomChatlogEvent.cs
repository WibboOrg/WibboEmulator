using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Chat.Logs;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorRoomChatlogEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            Packet.PopInt(); //useless
            int roomID = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(roomID);
            if (room == null)
            {
                return;
            }

            List<ChatlogEntry> listReverse = new List<ChatlogEntry>();
            listReverse.AddRange(room.GetChatMessageManager().ListOfMessages);
            listReverse.Reverse();

            Session.SendPacket(new ModeratorRoomChatlogComposer(room, listReverse));
        }
    }
}
