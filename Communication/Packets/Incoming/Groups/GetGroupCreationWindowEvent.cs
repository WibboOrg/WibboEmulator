using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetGroupCreationWindowEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            List<RoomData> ValidRooms = new List<RoomData>();
            foreach (RoomData Data in Session.GetHabbo().UsersRooms)
            {
                if (Data.Group == null)
                {
                    ValidRooms.Add(Data);
                }
            }

            Session.SendPacket(new GroupCreationWindowComposer(ValidRooms));
        }
    }
}