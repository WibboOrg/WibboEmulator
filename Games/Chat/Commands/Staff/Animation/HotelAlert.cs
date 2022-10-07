namespace WibboEmulator.Games.Chat.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class HotelAlert : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var message = CommandManager.MergeParams(parameters, 1);
        if (session.Antipub(message, "<CMD>", room.Id))
        {
            return;
        }
        WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new BroadcastMessageAlertComposer(message + "\r\n" + "- " + session.GetUser().Username));
    }
}
