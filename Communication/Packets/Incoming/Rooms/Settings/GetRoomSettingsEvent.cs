using Wibbo.Communication.Packets.Outgoing.Rooms.Settings;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetRoomSettingsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room Room = WibboEnvironment.GetGame().GetRoomManager().LoadRoom(Packet.PopInt());
            if (Room == null)
            {
                return;
            }

            if (!Room.CheckRights(Session, true) && !Session.GetUser().HasFuse("fuse_settings_room"))
            {
                return;
            }

            Session.SendPacket(new RoomSettingsDataComposer(Room.RoomData));
        }
    }
}