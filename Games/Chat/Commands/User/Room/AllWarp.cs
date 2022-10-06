namespace WibboEmulator.Games.Chat.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal class AllWarp : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var MessageList = new ServerPacketList();

        foreach (var user in Room.GetRoomUserManager().GetUserList().ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            MessageList.Add(Room.GetRoomItemHandler().TeleportUser(user, UserRoom.Coordinate, 0, Room.GetGameMap().SqAbsoluteHeight(UserRoom.X, UserRoom.Y)));
        }

        Room.SendMessage(MessageList);
    }
}
