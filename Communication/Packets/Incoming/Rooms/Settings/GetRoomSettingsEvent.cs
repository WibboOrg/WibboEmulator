using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null)
            {
                return;
            }

            if (!Room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("fuse_settings_room"))
            {
                return;
            }

            Session.SendPacket(new RoomSettingsDataComposer(Room.RoomData));
        }
    }
}