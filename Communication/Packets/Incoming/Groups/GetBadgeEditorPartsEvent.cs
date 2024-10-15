namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

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
            GroupManager.BadgeBases,
            GroupManager.BadgeSymbols,
            GroupManager.BadgeBaseColours,
            GroupManager.BadgeSymbolColours,
            GroupManager.BadgeBackColours));
    }
}
