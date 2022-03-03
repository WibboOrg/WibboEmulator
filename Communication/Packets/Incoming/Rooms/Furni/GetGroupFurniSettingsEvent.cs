using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Items;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetGroupFurniSettingsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || !Session.GetUser().InRoom)
            {
                return;
            }

            int ItemId = Packet.PopInt();
            int GroupId = Packet.PopInt();

            Item Item = Session.GetUser().CurrentRoom.GetRoomItemHandler().GetItem(ItemId);
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

            Session.SendPacket(new GroupFurniSettingsComposer(Group, ItemId, Session.GetUser().Id));
            Session.SendPacket(new GroupInfoComposer(Group, Session, false));
        }
    }
}