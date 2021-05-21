using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UnbanUserFromRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
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