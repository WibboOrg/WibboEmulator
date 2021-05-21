using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetFurnitureAliasesMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            ServerPacket Response = new ServerPacket(ServerPacketHeader.FURNITURE_ALIASES);
            Response.WriteInteger(0);
            Session.SendPacket(Response);
        }
    }
}