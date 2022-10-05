namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class About : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params) =>
        /// TimeSpan Uptime = DateTime.Now - ButterflyEnvironment.ServerStarted;

        session.SendPacket(new BroadcastMessageAlertComposer("<b>Butterfly Edition Wibbo</b>\n\n" +
            "   <b>Credits</b>:\n" +
            "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
            "   Mike, Sledmore, Joopie, Tweeny, \n" +
            "   JasonDhose, Leenster, Moogly, Niels, AKllX, rbi0s\n\n"));
}
