using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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
