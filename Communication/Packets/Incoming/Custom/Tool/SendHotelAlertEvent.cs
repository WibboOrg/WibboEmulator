using Butterfly.Communication.Packets.Outgoing.Custom;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Custom
{
    internal class SendHotelAlertEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (!Session.GetUser().HasFuse("fuse_wibbotool"))
            {
                return;
            }

            bool EventAlert = Packet.PopBoolean();
            string Message = Packet.PopString();
            string Url = Packet.PopString();
            bool Preview = Packet.PopBoolean();

            if (string.IsNullOrWhiteSpace(Message) || Message.Length > 1000 || Message.Length < 20)
            {
                return;
            }

            if (Preview)
            {
                Session.SendPacket(new NotifAlertComposer("staff", "Message de teste", Message, "Super !", 0, Url));
                return;
            }

            if (!string.IsNullOrWhiteSpace(Url))
            {
                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, 0, string.Empty, "hal", string.Format("WbTool hal: {0} : {1}", Url, Message));

                if (!Url.StartsWith("https://wibbo.org") && !Url.StartsWith("https://www.facebook.com/WibboHotelFR") && !Url.StartsWith("https://twitter.com/WibboOrg") && !Url.StartsWith("https://instagram.com/wibboorg"))
                {
                    return;
                }

                ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new NotifAlertComposer("annonce", "Message de communication", Message, "Allez voir !", 0, Url));
                return;
            }

            if (EventAlert)
            {
                if (Session.GetUser().CurrentRoom == null)
                {
                    return;
                }

                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, Session.GetUser().CurrentRoom.Id, string.Empty, "eventha", string.Format("WbTool eventha: {0}", Message));
                if (Session.Antipub(Message, "<eventalert>"))
                {
                    return;
                }

                if (!ButterflyEnvironment.GetGame().GetAnimationManager().AllowAnimation())
                {
                    Session.SendNotification("Merci d'attendre que l'animation en cours soit terminer");
                    return;
                }

                ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new NotifAlertComposer("game_promo_small", "Message d'animation", Message, "Je veux y jouer !", Session.GetUser().CurrentRoom.Id, ""));

                Session.GetUser().CurrentRoom.CloseFullRoom = true;
            }
            else
            {
                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetUser().Id, Session.GetUser().Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", Message));
                if (Session.Antipub(Message, "<alert>"))
                {
                    return;
                }

                ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new NotifAlertComposer("staff", "Message des Staffs", Message, "Compris !", 0, ""));
            }

        }
    }
}
