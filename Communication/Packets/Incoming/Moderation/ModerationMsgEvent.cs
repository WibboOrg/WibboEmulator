using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModerationMsgEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int userId = Packet.PopInt();
            string message = Packet.PopString();

            GameClient client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null)
                return;

            client.SendNotification(message);
        }
    }
}