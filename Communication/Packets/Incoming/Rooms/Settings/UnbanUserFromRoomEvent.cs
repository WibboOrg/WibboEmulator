using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class UnbanUserFromRoomEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client session, ClientPacket packet)
        {
            if (!session.GetUser().InRoom)
            {
                return;
            }

            Room Instance = session.GetUser().CurrentRoom;
            if (Instance == null || !Instance.CheckRights(session, true))
            {
                return;
            }

            int UserId = packet.PopInt();
            int RoomId = packet.PopInt();

            if (!Instance.HasBanExpired(UserId))
            {
                Instance.RemoveBan(UserId);

                session.SendPacket(new UnbanUserFromRoomComposer(RoomId, UserId));
            }
        }
    }
}