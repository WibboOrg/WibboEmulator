using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            Session.SendPacket(new BadgeEditorPartsComposer(
                ButterflyEnvironment.GetGame().GetGroupManager().BadgeBases,
                ButterflyEnvironment.GetGame().GetGroupManager().BadgeSymbols,
                ButterflyEnvironment.GetGame().GetGroupManager().BadgeBaseColours,
                ButterflyEnvironment.GetGame().GetGroupManager().BadgeSymbolColours,
                ButterflyEnvironment.GetGame().GetGroupManager().BadgeBackColours));
        }
    }
}
