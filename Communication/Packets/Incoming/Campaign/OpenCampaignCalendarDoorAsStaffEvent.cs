using Wibbo.Communication.Packets.Outgoing.Campaign;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Campaign
{
    internal class OpenCampaignCalendarDoorAsStaffEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string campaignName = Packet.PopString();
            int campaingnId = Packet.PopInt();

            Session.SendPacket(new CampaignCalendarDoorOpenedComposer(true, "", "", ""));
        }
    }
}
