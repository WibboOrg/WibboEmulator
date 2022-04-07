using Butterfly.Game.Clients;
using System;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Trigger : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.IsMuted)
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            RoomUser TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(Convert.ToString(Params[1]));

            if (TargetRoomUser == null || TargetRoomUser.GetClient() == null || TargetRoomUser.GetClient().GetUser() == null)
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetUser().Id == Session.GetUser().Id)
            {
                return;
            }

            if (!((Math.Abs((TargetRoomUser.X - UserRoom.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - UserRoom.Y)) >= 2)))
            {
                Room.onTriggerUser(TargetRoomUser, true);
                Room.onTriggerUser(UserRoom, false);
            }
        }
    }
}
