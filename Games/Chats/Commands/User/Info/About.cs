namespace WibboEmulator.Games.Chats.Commands.User.Info;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class About : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters) =>
        Session.SendPacket(new BroadcastMessageAlertComposer("<b>Butterfly Edition Wibbo</b>\n\n" +
            "   <b>Credits</b>:\n" +
            "   Meth0d, Matinmine, Carlos, Super0ca,\n" +
            "   Mike, Sledmore, Joopie, Tweeny, \n" +
            "   JasonDhose, Leenster, Moogly, Niels, AKllX, rbi0s\n\n"));
}
