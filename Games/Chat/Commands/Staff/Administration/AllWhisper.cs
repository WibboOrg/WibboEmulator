namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class AllWhisper : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length < 2)
        {
            return;
        }

        var Message = CommandManager.MergeParams(Params, 1);

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