using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class ModeratorActionEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_alert"))
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

            ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, AlertMessage.Split(' ')[0].Replace(":", ""), string.Format("Modtool Roomalert: {0}", AlertMessage));

            Session.GetHabbo().CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(AlertMessage));
        }
    }
}
