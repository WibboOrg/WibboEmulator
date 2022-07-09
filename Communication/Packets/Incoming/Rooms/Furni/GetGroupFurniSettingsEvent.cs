using Wibbo.Communication.Packets.Outgoing.Groups;
using Wibbo.Game.Clients;
using Wibbo.Game.Groups;
using Wibbo.Game.Items;

namespace Wibbo.Communication.Packets.Incoming.Structure
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

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.SendPacket(new GroupFurniSettingsComposer(Group, ItemId, Session.GetUser().Id));
            Session.SendPacket(new GroupInfoComposer(Group, Session, false));
        }
    }
}