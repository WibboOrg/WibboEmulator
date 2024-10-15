namespace WibboEmulator.Communication.Packets.Incoming.LandingView;
using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Games.GameClients;

internal sealed class RefreshCampaignEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var parseCampaings = packet.PopString();
        if (parseCampaings.Contains("gamesmaker"))
        {
            return;
        }

        var campaingName = "";
        var parser = parseCampaings.Split(';');

        for (var i = 0; i < parser.Length; i++)
        {
            if (string.IsNullOrEmpty(parser[i]) || parser[i].EndsWith(','))
            {
                continue;
            }

            var data = parser[i].Split(',');
            campaingName = data[1];
        }

        session.SendPacket(new CampaignComposer(parseCampaings, campaingName));
    }
}
