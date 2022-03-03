using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModeratorActionEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_alert"))
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

            ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, 0, string.Empty, AlertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", AlertMessage));

            Session.GetUser().CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(AlertMessage));
        }
    }
}
