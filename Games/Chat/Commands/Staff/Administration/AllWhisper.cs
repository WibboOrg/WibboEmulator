namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllWhisper : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length < 2)
        {
            return;
        }

        var Message = CommandManager.MergeParams(parameters, 1);

        foreach (var User in Room.GetRoomUserManager().GetUserList().ToList())
        {
            if (User == null || User.GetClient() == null)
            {
                continue;
            }
            User.GetClient().SendPacket(new WhisperComposer(UserRoom.VirtualId, Message, 0));
        }
    }
}