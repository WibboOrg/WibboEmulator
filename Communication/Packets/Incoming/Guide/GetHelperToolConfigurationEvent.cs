using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Game.Clients;
using Butterfly.Game.Help;

namespace Butterfly.Communication.Packets.Incoming.Guide
{
    internal class GetHelperToolConfigurationEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().HasFuse("fuse_helptool"))
            {
                return;
            }

            HelpManager guideManager = ButterflyEnvironment.GetGame().GetHelpManager();
            bool onDuty = Packet.PopBoolean();
            Packet.PopBoolean();
            Packet.PopBoolean();
            Packet.PopBoolean();

            if (onDuty && !Session.GetUser().OnDuty)
            {
                guideManager.AddGuide(Session.GetUser().Id);
                Session.GetUser().OnDuty = true;
            }
            else
            {
                guideManager.RemoveGuide(Session.GetUser().Id);
                Session.GetUser().OnDuty = false;
            }

            Session.SendPacket(new HelperToolComposer(Session.GetUser().OnDuty, guideManager.GuidesCount));
        }
    }
}