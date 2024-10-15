namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class GetBadgeEditorPartsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        Session.SendPacket(new BadgeEditorPartsComposer(
            GroupManager.BadgeBases,
            GroupManager.BadgeSymbols,
            GroupManager.BadgeBaseColours,
            GroupManager.BadgeSymbolColours,
            GroupManager.BadgeBackColours));
    }
}
