namespace WibboEmulator.Communication.Packets.Incoming.Campaign;
using WibboEmulator.Communication.Packets.Outgoing.Campaign;
using WibboEmulator.Games.GameClients;

internal class OpenCampaignCalendarDoorEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var campaignName = Packet.PopString();
        var campaingnId = Packet.PopInt();

        session.SendPacket(new CampaignCalendarDoorOpenedComposer(true, "", "/album1584/LOL.gif", ""));
    }
}
