using WibboEmulator.Communication.Packets.Outgoing.LandingView;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RefreshCampaignEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string parseCampaings = Packet.PopString();
            if (parseCampaings.Contains("gamesmaker"))
            {
                return;
            }

            string campaingName = "";
            string[] parser = parseCampaings.Split(';');

            for (int i = 0; i < parser.Length; i++)
            {
                if (string.IsNullOrEmpty(parser[i]) || parser[i].EndsWith(","))
                {
                    continue;
                }

                string[] data = parser[i].Split(',');
                campaingName = data[1];
            }

            Session.SendPacket(new CampaignComposer(parseCampaings, campaingName));
        }
    }
}
