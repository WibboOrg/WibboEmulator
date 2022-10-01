using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class ModeratorActionEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasPermission("perm_alert"))
            {
                return;
            }

            int AlertMode = Packet.PopInt();
            string AlertMessage = Packet.PopString();
            bool IsCaution = AlertMode != 3;

            if (Session.Antipub(AlertMessage, "<MT>"))
            {
                return;
            }

            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, 0, string.Empty, AlertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", AlertMessage));

            Session.GetUser().CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(AlertMessage));
        }
    }
}
