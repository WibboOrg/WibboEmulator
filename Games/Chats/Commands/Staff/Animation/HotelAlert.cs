namespace WibboEmulator.Games.Chats.Commands.Staff.Animation;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class HotelAlert : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var message = CommandManager.MergeParams(parameters, 1);
        if (Session.User.CheckChatMessage(message, "<CMD>", room.Id))
        {
            return;
        }
        GameClientManager.SendMessage(new BroadcastMessageAlertComposer(message + "\r\n" + "- " + Session.User.Username));
    }
}
