namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllWhisper : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var message = CommandManager.MergeParams(parameters, 1);

        foreach (var user in room.RoomUserManager.GetUserList().ToList())
        {
            if (user == null || user.Client == null)
            {
                continue;
            }
            user.Client.SendPacket(new WhisperComposer(userRoom.VirtualId, message, 0));
        }
    }
}
