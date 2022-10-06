namespace WibboEmulator.Communication.Packets.Incoming.WibboTool;

using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Games.GameClients;

internal class SendHotelAlertEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        if (!session.GetUser().HasPermission("perm_wibbotool"))
        {
            return;
        }

        var EventAlert = packet.PopBoolean();
        var Message = packet.PopString();
        var Url = packet.PopString();
        var Preview = packet.PopBoolean();

        if (string.IsNullOrWhiteSpace(Message) || Message.Length > 2000 || Message.Length < 50)
        {
            return;
        }

        if (Preview)
        {
            session.SendPacket(new NotifAlertComposer("staff", "Message de prévisualisation", Message, "Super !", 0, Url));
            return;
        }

        if (!string.IsNullOrWhiteSpace(Url))
        {
            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(session.GetUser().Id, session.GetUser().Username, 0, string.Empty, "hal", string.Format("WibbbTool HaL: {0} : {1}", Url, Message));

            if (!Url.StartsWith("https://wibbo.org") && !Url.StartsWith("https://www.facebook.com/WibboHotelFR") && !Url.StartsWith("https://twitter.com/WibboOrg") && !Url.StartsWith("https://instagram.com/wibboorg"))
            {
                return;
            }

            WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer("annonce", "Message de communication", Message, "Allez voir !", 0, Url));
            return;
        }

        if (EventAlert)
        {
            if (session.GetUser().CurrentRoom == null)
            {
                return;
            }

            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(session.GetUser().Id, session.GetUser().Username, session.GetUser().CurrentRoom.Id, string.Empty, "eventha", string.Format("WibbobTool EventHa: {0}", Message));
            if (session.Antipub(Message, "<eventalert>"))
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetAnimationManager().AllowAnimation())
            {
                session.SendNotification("Vous pourrez lancer l'alerte une fois que l'animation en cours sera terminé.");
                return;
            }

            WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer("game_promo_small", "Message d'animation", Message, "Je veux y jouer !", session.GetUser().CurrentRoom.Id, ""));

            session.GetUser().CurrentRoom.CloseFullRoom = true;
        }
        else
        {
            WibboEnvironment.GetGame().GetModerationManager().LogStaffEntry(session.GetUser().Id, session.GetUser().Username, 0, string.Empty, "ha", string.Format("WbTool ha: {0}", Message));
            if (session.Antipub(Message, "<alert>"))
            {
                return;
            }

            WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifAlertComposer("staff", "Message de l'Équipe", Message, "Compris !", 0, ""));
        }

    }
}
