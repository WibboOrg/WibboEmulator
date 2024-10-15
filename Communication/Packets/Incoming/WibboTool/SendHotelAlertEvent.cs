namespace WibboEmulator.Communication.Packets.Incoming.WibboTool;

using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Games.Animations;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderations;

internal sealed class SendHotelAlertEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        if (!Session.User.HasPermission("wibbotool"))
        {
            return;
        }

        var eventAlert = packet.PopBoolean();
        var message = packet.PopString(2000);
        var url = packet.PopString();
        var preview = packet.PopBoolean();

        if (string.IsNullOrWhiteSpace(message) || message.Length > 2000 || message.Length < 50)
        {
            return;
        }

        if (preview)
        {
            Session.SendPacket(new NotifAlertComposer("staff", "Message de prévisualisation", message, "Super !", 0, url));
            return;
        }

        if (!string.IsNullOrWhiteSpace(url))
        {
            ModerationManager.LogStaffEntry(Session.User.Id, Session.User.Username, 0, string.Empty, "hal", string.Format("Tool HaL: {0} : {1}", url, message));

            // if (!url.StartsWith("https://wibbo.org") && !url.StartsWith("https://www.facebook.com/WibboHotelFR") && !url.StartsWith("https://twitter.com/WibboOrg") && !url.StartsWith("https://instagram.com/wibboorg"))
            // {
            //     return;
            // }

            GameClientManager.SendMessage(new NotifAlertComposer("annonce", "Message de communication", message, "Allez voir !", 0, url));
            return;
        }

        if (eventAlert)
        {
            if (Session.User.Room == null)
            {
                return;
            }

            ModerationManager.LogStaffEntry(Session.User.Id, Session.User.Username, Session.User.Room.Id, string.Empty, "eventha", string.Format("WibbobTool EventHa: {0}", message));
            //if (Session.User.Antipub(message, "<eventalert>", Session.User.CurrentRoom.Id))
            //{
            //return;
            //}

            if (!AnimationManager.AllowAnimation)
            {
                Session.SendNotification("Vous pourrez lancer l'alerte une fois que l'animation en cours sera terminé.");
                return;
            }

            GameClientManager.SendMessage(new NotifAlertComposer("game_promo_small", "Message d'animation", message, "Je veux y jouer !", Session.User.Room.Id, ""));

            Session.User.Room.CloseFullRoom = true;
        }
        else
        {
            ModerationManager.LogStaffEntry(Session.User.Id, Session.User.Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", message));
            if (Session.User.CheckChatMessage(message, "<alert>"))
            {
                return;
            }

            GameClientManager.SendMessage(new NotifAlertComposer("staff", "Message de l'Équipe", message, "Compris !", 0, ""));
        }

    }
}
