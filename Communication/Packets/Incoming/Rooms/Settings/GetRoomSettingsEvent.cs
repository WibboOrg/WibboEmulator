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
            int roomId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out Room room))
                return;

            if (!room.CheckRights(Session, true) && !Session.GetUser().HasPermission("perm_settings_room"))
            {
                return;
            }

            Session.SendPacket(new RoomSettingsDataComposer(room.RoomData));
        }
    }
}