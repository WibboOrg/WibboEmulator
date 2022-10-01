using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ModerationMsgEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_alert"))
            {
                return;
            }

            int userId = Packet.PopInt();
            string message = Packet.PopString();

            GameClient clientTarget = WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (clientTarget == null)
                return;

            if (clientTarget.GetUser().Id == Session.GetUser().Id)
            {
                return;
            }

            if (clientTarget.GetUser().Rank >= Session.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("moderation.caution.missingrank", Session.Langue));
            }

            if (Session.Antipub(message, "<MT>"))
            {
                return;
            }

            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, 0, string.Empty, "ModTool", string.Format("Modtool alert ( {1} ): {0}", message, clientTarget.GetUser().Username));

            clientTarget.SendNotification(message);
        }
    }
}