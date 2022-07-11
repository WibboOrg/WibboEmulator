using WibboEmulator.Communication.Packets.Outgoing.Campaign;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Campaign
{
    internal class OpenCampaignCalendarDoorEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string campaignName = Packet.PopString();
            int campaingnId = Packet.PopInt();

            Session.SendPacket(new CampaignCalendarDoorOpenedComposer(true, "", "/album1584/LOL.gif", ""));
        }
    }
}
