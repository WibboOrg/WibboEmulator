namespace WibboEmulator.Communication.Packets.Incoming.WibboTool;

using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Moderation;

internal class SendHotelAlertEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        if (!session.User.HasPermission("perm_wibbotool"))
        {
            return;
        }

        var eventAlert = packet.PopBoolean();
        var message = packet.PopString();
        var url = packet.PopString();
        var preview = packet.PopBoolean();

        if (string.IsNullOrWhiteSpace(message) || message.Length > 2000 || message.Length < 50)
        {
            return;
        }

        if (preview)
        {
            session.SendPacket(new NotifAlertComposer("staff", "Message de prévisualisation", message, "Super !", 0, url));
            return;
        }

        if (!string.IsNullOrWhiteSpace(url))
        {
            ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, 0, string.Empty, "hal", string.Format("WibbbTool HaL: {0} : {1}", url, message));

            if (!url.StartsWith("https://wibbo.org") && !url.StartsWith("https://www.facebook.com/WibboHotelFR") && !url.StartsWith("https://twitter.com/WibboOrg") && !url.StartsWith("https://instagram.com/wibboorg"))
            {
                return;
            }

            WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer("annonce", "Message de communication", message, "Allez voir !", 0, url));
            return;
        }

        if (eventAlert)
        {
            if (session.User.CurrentRoom == null)
            {
                return;
            }

            ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, session.User.CurrentRoom.Id, string.Empty, "eventha", string.Format("WibbobTool EventHa: {0}", message));
            if (session.Antipub(message, "<eventalert>"))
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
            {
                session.SendNotification("Vous pourrez lancer l'alerte une fois que l'animation en cours sera terminé.");
                return;
            }

            WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer("game_promo_small", "Message d'animation", message, "Je veux y jouer !", session.User.CurrentRoom.Id, ""));

            session.User.CurrentRoom.CloseFullRoom = true;
        }
        else
        {
            ModerationManager.LogStaffEntry(session.User.Id, session.User.Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", message));
            if (session.Antipub(message, "<alert>"))
            {
                return;
            }

            WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer("staff", "Message de l'Équipe", message, "Compris !", 0, ""));
        }

    }
}
