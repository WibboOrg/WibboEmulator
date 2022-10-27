namespace WibboEmulator.Games.Chats.Commands.User.Info;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Info : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var uptime = DateTime.Now - WibboEnvironment.ServerStarted;

        var onlineUsers = WibboEnvironment.GetGame().GetGameClientManager().Count;
        var roomCount = WibboEnvironment.GetGame().GetRoomManager().Count;

        session.SendPacket(new BroadcastMessageAlertComposer("<b>Butterfly Edition Wibbo</b>\n\n" +
             "   <b>Credits</b>:\n" +
             "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
             "   Mike, Sledmore, Joopie, Tweeny, \n" +
             "   JasonDhose, Leenster, Moogly, Niels, AKllX, rbi0s\n\n" +
             "   <b>Information sur le serveur</b>:\n" +
             "   Joueurs en ligne: " + onlineUsers + "\n" +
             "   Appartements actifs: " + roomCount + "\n" +
             "   Uptime: " + uptime.Days + " jour(s), " + uptime.Hours + " heure(s) et " + uptime.Minutes + " minutes.\n\n"));
    }
}
