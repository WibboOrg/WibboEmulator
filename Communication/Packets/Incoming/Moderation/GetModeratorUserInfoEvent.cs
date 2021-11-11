using Butterfly.Game.GameClients;
using Butterfly.Game.Moderation;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetModeratorUserInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                return;
            }

            int num = Packet.PopInt();
            if (ButterflyEnvironment.GetGame().GetClientManager().GetNameById(num) != "")
            {
                Session.SendPacket(ModerationManager.SerializeUserInfo(num));
            }
            else
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.loadusererror", Session.Langue));
            }
        }
    }
}
