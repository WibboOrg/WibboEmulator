using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModerationMsgEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
            {
                return;
            }

            int userId = Packet.PopInt();
            string message = Packet.PopString();

            Client clientTarget = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (clientTarget == null)
                return;

            if (clientTarget.GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (clientTarget.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("moderation.caution.missingrank", Session.Langue));
            }

            if (Session.Antipub(message, "<MT>"))
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "ModTool", string.Format("Modtool alert ( {1} ): {0}", message, clientTarget.GetHabbo().Username));

            clientTarget.SendNotification(message);
        }
    }
}