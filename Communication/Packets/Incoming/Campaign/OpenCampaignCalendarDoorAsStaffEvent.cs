namespace WibboEmulator.Communication.Packets.Incoming.Campaign;
using WibboEmulator.Communication.Packets.Outgoing.Campaign;
using WibboEmulator.Games.GameClients;

internal sealed class OpenCampaignCalendarDoorAsStaffEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        _ = packet.PopString();

        _ = packet.PopInt();

        session.SendPacket(new CampaignCalendarDoorOpenedComposer(true, "", "", ""));
    }
}
