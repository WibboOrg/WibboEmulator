using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetClientVersionEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string Release = Packet.PopString();
            string Type = Packet.PopString();
            int Platform = Packet.PopInt();
            int Category = Packet.PopInt();

            if (Release != "PRODUCTION-201611291003-338511768")
            {
                return;
            }

            if (Type != "FLASH" || Platform != 1 || Category != 0)
            {
                return;
            }
        }
    }
}
