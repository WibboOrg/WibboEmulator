using Wibbo.Communication.Packets.Outgoing.Help;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Guide
{
    internal class RecomendHelpersEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new OnGuideSessionDetachedComposer());
        }
    }
}
