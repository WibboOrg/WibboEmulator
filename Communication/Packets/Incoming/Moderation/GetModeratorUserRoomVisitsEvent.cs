using Butterfly.HabboHotel.GameClients;
using Butterfly.Communication.Packets.Outgoing.Moderation;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserRoomVisitsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int userId = Packet.PopInt();

            GameClient clientTarget = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);

            if(clientTarget == null)
            {
                return;
            }

            Session.SendPacket(new ModeratorUserRoomVisitsComposer(clientTarget.GetHabbo(), clientTarget.GetHabbo().Visits));
        }
    }
}
