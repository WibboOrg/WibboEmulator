namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Info : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var Uptime = DateTime.Now - WibboEnvironment.ServerStarted;

        var OnlineUsers = WibboEnvironment.GetGame().GetGameClientManager().Count;
        var RoomCount = WibboEnvironment.GetGame().GetRoomManager().Count;

        session.SendPacket(new BroadcastMessageAlertComposer("<b>Butterfly Edition Wibbo</b>\n\n" +
             "   <b>Credits</b>:\n" +
             "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
             "   Mike, Sledmore, Joopie, Tweeny, \n" +
             "   JasonDhose, Leenster, Moogly, Niels, AKllX, rbi0s\n\n" +
             "   <b>Information sur le serveur</b>:\n" +
             "   Joueurs en ligne: " + OnlineUsers + "\n" +
             "   Appartements actifs: " + RoomCount + "\n" +
             "   Uptime: " + Uptime.Days + " jour(s), " + Uptime.Hours + " heure(s) et " + Uptime.Minutes + " minutes.\n\n"));
    }
}