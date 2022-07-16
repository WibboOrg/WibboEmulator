using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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

            if (!Room.CheckRights(Session, true) && !Session.GetUser().HasPermission("perm_"))
            {
                return;
            }

            Session.SendPacket(new RoomSettingsDataComposer(Room.RoomData));
        }
    }
}