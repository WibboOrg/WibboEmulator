namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;

using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class NotifTop : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var Message = CommandManager.MergeParams(parameters, 1);
        if (string.IsNullOrEmpty(Message))
        {
            return;
        }

        WibboEnvironment.GetGame().GetGameClientManager().SendMessage(new NotifTopComposer(Message));
    }
}