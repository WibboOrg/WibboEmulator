using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetGroupFurniSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom)
            {
                return;
            }

            int ItemId = Packet.PopInt();
            int GroupId = Packet.PopInt();

            Item Item = Session.GetHabbo().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
            {
                return;
            }

            if (Item.Data.InteractionType != InteractionType.GUILD_GATE)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.SendPacket(new GroupFurniSettingsComposer(Group, ItemId, Session.GetHabbo().Id));
            Session.SendPacket(new GroupInfoComposer(Group, Session, false));
        }
    }
}