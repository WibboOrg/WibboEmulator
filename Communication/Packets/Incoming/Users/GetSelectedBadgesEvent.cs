using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using Butterfly.Game.Users.Badges;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Packet.PopInt());
            if (roomUserByHabbo == null || roomUserByHabbo.IsBot || roomUserByHabbo.GetClient() == null || roomUserByHabbo.GetClient().GetHabbo() == null || roomUserByHabbo.GetClient().GetHabbo().GetBadgeComponent() == null)
            {
                return;
            }

            User HabboTarget = roomUserByHabbo.GetClient().GetHabbo();

            Session.SendPacket(new HabboUserBadgesComposer(HabboTarget.Id, HabboTarget.GetBadgeComponent().EquippedCount, HabboTarget.GetBadgeComponent().BadgeList));
        }
    }
}