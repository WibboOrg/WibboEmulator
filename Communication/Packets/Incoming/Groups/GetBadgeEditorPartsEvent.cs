using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            Session.SendPacket(new BadgeEditorPartsComposer(
                WibboEnvironment.GetGame().GetGroupManager().BadgeBases,
                WibboEnvironment.GetGame().GetGroupManager().BadgeSymbols,
                WibboEnvironment.GetGame().GetGroupManager().BadgeBaseColours,
                WibboEnvironment.GetGame().GetGroupManager().BadgeSymbolColours,
                WibboEnvironment.GetGame().GetGroupManager().BadgeBackColours));
        }
    }
}
