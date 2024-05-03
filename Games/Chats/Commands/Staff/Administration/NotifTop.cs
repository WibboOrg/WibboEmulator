namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class NotifTop : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var message = CommandManager.MergeParams(parameters, 1);
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        GameClientManager.SendMessage(new NotifTopComposer(message));
    }
}
