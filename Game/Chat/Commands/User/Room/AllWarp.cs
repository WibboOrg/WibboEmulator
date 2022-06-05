using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;
using Wibbo.Utilities;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class AllWarp : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetUser().CurrentRoom;
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            ServerPacketList MessageList = new ServerPacketList();

            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (user == null || user.IsBot)
                {
                    continue;
                }

                MessageList.Add(room.GetRoomItemHandler().TeleportUser(user, UserRoom.Coordinate, 0, room.GetGameMap().SqAbsoluteHeight(UserRoom.X, UserRoom.Y)));
            }

            room.SendMessage(MessageList);
        }
    }
}
