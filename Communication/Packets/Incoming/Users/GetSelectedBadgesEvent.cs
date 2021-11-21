using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;
using Butterfly.Game.User.Badges;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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

            ServerPacket Response = new ServerPacket(ServerPacketHeader.USER_BADGES_CURRENT);
            Response.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().Id);
            Response.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().GetBadgeComponent().EquippedCount);

            int BadgeCount = 0;
            foreach (Badge badge in roomUserByHabbo.GetClient().GetHabbo().GetBadgeComponent().BadgeList.Values)
            {
                if (badge.Slot > 0)
                {
                    BadgeCount++;
                    if (BadgeCount > 5)
                    {
                        break;
                    }

                    Response.WriteInteger(badge.Slot);
                    Response.WriteString(badge.Code);
                }
            }

            Session.SendPacket(Response);
        }
    }
}