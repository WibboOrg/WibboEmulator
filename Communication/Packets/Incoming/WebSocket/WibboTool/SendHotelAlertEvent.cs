using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.WebClients;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class SendHotelAlertEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            if (Session == null)
            {
                return;
            }

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
            {
                return;
            }

            if (!Client.GetHabbo().HasFuse("fuse_wibbotool"))
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
                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, 0, string.Empty, "hal", string.Format("WbTool hal: {0} : {1}", Url, Message));

                if (!Url.StartsWith("https://wibbo.org") && !Url.StartsWith("https://www.facebook.com/WibboHotelFR") && !Url.StartsWith("https://twitter.com/WibboOrg") && !Url.StartsWith("https://instagram.com/wibboorg"))
                {
                    return;
                }

                ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("annonce", "Message de communication", Message, "Allez voir !", 0, Url), Session.Langue);
                return;
            }

            if (EventAlert)
            {
                if (Client.GetHabbo().CurrentRoom == null)
                {
                    return;
                }

                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, Client.GetHabbo().CurrentRoom.Id, string.Empty, "eventha", string.Format("WbTool eventha: {0}", Message));
                if (Client.Antipub(Message, "<eventalert>"))
                {
                    return;
                }

                if (!ButterflyEnvironment.GetGame().GetAnimationManager().AllowAnimation())
                {
                    Client.SendNotification("Merci d'attendre que l'animation en cours soit terminer");
                    return;
                }


                //ButterflyEnvironment.GetGame().GetClientManager().SendSuperNotif("Message des Staffs", AlertMessage, "game_promo_small", "event:navigator/goto/" + Client.GetHabbo().CurrentRoom.Id, "Je veux y accéder!", true, true);
                ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("game_promo_small", "Message d'animation", Message, "Je veux y jouer !", Client.GetHabbo().CurrentRoom.Id, ""), Session.Langue);

                Client.GetHabbo().CurrentRoom.CloseFullRoom = true;
            }
            else
            {
                ButterflyEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", Message));
                if (Client.Antipub(Message, "<alert>"))
                {
                    return;
                }

                //ServerPacket message = new ServerPacket(ServerPacketHeader.BroadcastMessageAlertMessageComposer);
                //message.WriteString(ButterflyEnvironment.GetLanguageManager().TryGetValue("hotelallert.notice", Client.Langue) + "\r\n" + Message);// + "\r\n- " + Client.GetHabbo().Username);
                //ButterflyEnvironment.GetGame().GetClientManager().SendMessage(message);
                ButterflyEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("staff", "Message des Staffs", Message, "Compris !", 0, ""), Session.Langue);
            }

        }
    }
}
