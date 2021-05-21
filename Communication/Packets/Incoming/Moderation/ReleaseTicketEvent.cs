using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ReleaseTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int num = Packet.PopInt();
            for (int index = 0; index < num; ++index)
            {
                ButterflyEnvironment.GetGame().GetModerationManager().ReleaseTicket(Session, Packet.PopInt());
            }
        }
    }
}
