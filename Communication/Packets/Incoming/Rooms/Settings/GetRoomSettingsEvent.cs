using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
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