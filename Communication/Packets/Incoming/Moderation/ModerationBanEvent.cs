using Wibbo.Game.Clients;
using Wibbo.Game.Moderation;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class ModerationBanEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_ban"))
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