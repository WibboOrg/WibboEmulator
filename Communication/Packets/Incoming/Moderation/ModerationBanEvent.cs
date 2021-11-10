using Butterfly.Game.GameClients;
using Butterfly.Game.Support;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModerationBanEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_ban"))
            {
                return;
            }

            int UserId = Packet.PopInt();
            string Message = Packet.PopString();
            int Length = Packet.PopInt() * 3600;

            ModerationManager.BanUser(Session, UserId, Length, Message);
        }
    }
}