using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetGroupFurniConfigEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new GroupFurniConfigComposer(ButterflyEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().MyGroups)));
        }
    }
}