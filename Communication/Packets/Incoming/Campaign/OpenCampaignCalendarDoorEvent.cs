using Wibbo.Communication.Packets.Outgoing.Campaign;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Campaign
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
