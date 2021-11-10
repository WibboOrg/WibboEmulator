using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class PickTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            Packet.PopInt();
            ButterflyEnvironment.GetGame().GetModerationManager().PickTicket(Session, Packet.PopInt());
        }
    }
}
