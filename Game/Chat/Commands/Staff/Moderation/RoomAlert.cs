using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class RoomAlert : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (RoomUser == null || RoomUser.GetClient() == null || Session.GetUser().Id == RoomUser.UserId)
                    continue;

                RoomUser.GetClient().SendNotification(Message);
            }
        }
    }
}