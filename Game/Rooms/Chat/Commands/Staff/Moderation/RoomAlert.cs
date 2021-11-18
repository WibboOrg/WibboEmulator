using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class RoomAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);
            foreach (RoomUser RoomUser in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (RoomUser == null || RoomUser.GetClient() == null || Session.GetHabbo().Id == RoomUser.UserId)
                    continue;

                RoomUser.GetClient().SendNotification(Message);
            }
        }
    }
}