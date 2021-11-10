using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InfoRetrieveEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.SendPacket(new UserObjectComposer(Session.GetHabbo()));
            Session.SendPacket(new UserPerksComposer(Session.GetHabbo()));
        }
    }
}
