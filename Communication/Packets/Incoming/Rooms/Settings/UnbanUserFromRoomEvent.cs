using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UnbanUserFromRoomEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
            {
                return;
            }

            Room Instance = session.GetHabbo().CurrentRoom;
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