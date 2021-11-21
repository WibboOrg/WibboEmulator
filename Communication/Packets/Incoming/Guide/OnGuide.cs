using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.Clients;
using Butterfly.Game.Help;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class OnGuide : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            int userId = Packet.PopInt();
            string message = Packet.PopString();

            HelpManager guideManager = ButterflyEnvironment.GetGame().GetHelpManager();
            if (guideManager.GuidesCount <= 0)
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.OnGuideSessionError);
                Response.WriteInteger(2);
                Session.SendPacket(Response);
                return;
            }

            if (Session.GetHabbo().OnDuty == true)
            {
                guideManager.RemoveGuide(Session.GetHabbo().Id);
            }

            int guideId = guideManager.GetRandomGuide();
            if (guideId == 0)
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.OnGuideSessionError);
                Response.WriteInteger(2);
                Session.SendPacket(Response);
                return;
            }

            Client guide = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(guideId);

            ServerPacket onGuideSessionAttached = new ServerPacket(ServerPacketHeader.OnGuideSessionAttached);
            onGuideSessionAttached.WriteBoolean(false);
            onGuideSessionAttached.WriteInteger(userId);
            onGuideSessionAttached.WriteString(message);
            onGuideSessionAttached.WriteInteger(30); //Temps moyen
            Session.SendPacket(onGuideSessionAttached);

            ServerPacket onGuideSessionAttached2 = new ServerPacket(ServerPacketHeader.OnGuideSessionAttached);
            onGuideSessionAttached2.WriteBoolean(true);
            onGuideSessionAttached2.WriteInteger(userId);
            onGuideSessionAttached2.WriteString(message);
            onGuideSessionAttached2.WriteInteger(15);
            guide.SendPacket(onGuideSessionAttached2);

            guide.GetHabbo().GuideOtherUserId = Session.GetHabbo().Id;
            Session.GetHabbo().GuideOtherUserId = guide.GetHabbo().Id;
        }
    }
}
