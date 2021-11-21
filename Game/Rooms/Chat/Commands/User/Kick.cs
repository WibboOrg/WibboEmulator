using Butterfly.Game.Clients;
using System.Linq;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Kick : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else
            {
                foreach (RoomUser User in currentRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (User != null && !User.IsBot && !User.GetClient().GetHabbo().HasFuse("fuse_mod") && User.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        User.AllowMoveTo = false;
                        User.IsWalking = true;
                        User.AllowOverride = true;
                        User.GoalX = Room.GetGameMap().Model.DoorX;
                        User.GoalY = Room.GetGameMap().Model.DoorY;
                    }
                }


                room.SetTimeout(3000, () =>
                {
                    foreach (RoomUser User in currentRoom.GetRoomUserManager().GetUserList().ToList())
                    {
                        if (User != null && !User.IsBot && !User.GetClient().GetHabbo().HasFuse("fuse_mod") && User.GetClient().GetHabbo().Id != Session.GetHabbo().Id)
                        {
                            Room.GetRoomUserManager().RemoveUserFromRoom(clientByUsername, true, true);
                        }
                    }
                });
            }
        }
    }
}