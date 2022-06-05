using Wibbo.Communication.Packets.Outgoing.Catalog;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class GetGroupFurniConfigEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new GroupFurniConfigComposer(WibboEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetUser().MyGroups)));
        }
    }
}