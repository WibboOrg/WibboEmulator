using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserRoomVisitsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int userId = Packet.PopInt();

            Client clientTarget = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);

            if (clientTarget == null)
            {
                return;
            }

            Session.SendPacket(new ModeratorUserRoomVisitsComposer(clientTarget.GetHabbo(), clientTarget.GetHabbo().Visits));
        }
    }
}
