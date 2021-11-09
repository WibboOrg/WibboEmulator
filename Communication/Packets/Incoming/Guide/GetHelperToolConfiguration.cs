using Butterfly.Communication.Packets.Outgoing;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Help;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetHelperToolConfiguration : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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

            ServerPacket HelpTool = new ServerPacket(ServerPacketHeader.HelperToolMessageComposer);
            HelpTool.WriteBoolean(Session.GetHabbo().OnDuty);
            HelpTool.WriteInteger(guideManager.GuidesCount);
            HelpTool.WriteInteger(0);
            HelpTool.WriteInteger(0);
            Session.SendPacket(HelpTool);
        }
    }
}