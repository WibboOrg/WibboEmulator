using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class CancellInviteGuide : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient requester = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);

            ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionDetached);
            Session.SendPacket(message);

            if (requester != null)
            {
                requester.SendPacket(message);
            }
        }
    }
}