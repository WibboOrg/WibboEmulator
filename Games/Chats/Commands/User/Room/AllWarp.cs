namespace WibboEmulator.Games.Chats.Commands.User.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal class AllWarp : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var messageList = new ServerPacketList();

        foreach (var user in room.RoomUserManager.GetUserList().ToList())
        {
            if (user == null || user.IsBot)
            {
                continue;
            }

            messageList.Add(RoomItemHandling.TeleportUser(user, userRoom.Coordinate, 0, room.GameMap.SqAbsoluteHeight(userRoom.X, userRoom.Y)));
        }

        room.SendMessage(messageList);
    }
}
