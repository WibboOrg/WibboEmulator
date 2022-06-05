using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class ReleaseTicketEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_mod"))
            {
                return;
            }

            int num = Packet.PopInt();
            for (int index = 0; index < num; ++index)
            {
                WibboEnvironment.GetGame().GetModerationManager().ReleaseTicket(Session, Packet.PopInt());
            }
        }
    }
}
