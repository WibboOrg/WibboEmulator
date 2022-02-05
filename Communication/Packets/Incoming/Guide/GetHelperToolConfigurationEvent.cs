using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;
using Butterfly.Game.Help;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetHelperToolConfigurationEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_helptool"))
            {
                return;
            }

            HelpManager guideManager = ButterflyEnvironment.GetGame().GetHelpManager();
            bool onDuty = Packet.PopBoolean();
            Packet.PopBoolean();
            Packet.PopBoolean();
            Packet.PopBoolean();

            if (onDuty && !Session.GetHabbo().OnDuty)
            {
                guideManager.AddGuide(Session.GetHabbo().Id);
                Session.GetHabbo().OnDuty = true;
            }
            else
            {
                guideManager.RemoveGuide(Session.GetHabbo().Id);
                Session.GetHabbo().OnDuty = false;
            }

            Session.SendPacket(new HelperToolComposer(Session.GetHabbo().OnDuty, guideManager.GuidesCount));
        }
    }
}