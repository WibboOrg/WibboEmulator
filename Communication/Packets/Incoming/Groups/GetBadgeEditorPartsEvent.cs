namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;

internal sealed class GetBadgeEditorPartsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        session.SendPacket(new BadgeEditorPartsComposer(
            WibboEnvironment.GetGame().GetGroupManager().BadgeBases,
            WibboEnvironment.GetGame().GetGroupManager().BadgeSymbols,
            WibboEnvironment.GetGame().GetGroupManager().BadgeBaseColours,
            WibboEnvironment.GetGame().GetGroupManager().BadgeSymbolColours,
            WibboEnvironment.GetGame().GetGroupManager().BadgeBackColours));
    }
}
