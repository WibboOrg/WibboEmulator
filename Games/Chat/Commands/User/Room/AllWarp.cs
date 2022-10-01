using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class AllWarp : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            ServerPacketList MessageList = new ServerPacketList();

            foreach (RoomUser user in Room.GetRoomUserManager().GetUserList().ToList())
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
}
