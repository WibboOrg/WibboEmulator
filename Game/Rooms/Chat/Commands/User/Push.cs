using Butterfly.Game.GameClients;
using System;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
            {
                return;
            }

            RoomUser TargetRoomUser;
            {
                return;
            }

            if (!TargetRoom.PushPullAllowed)
            {
                return;
            }

            TargetRoomUser1 = TargetRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            TargetRoomUser = TargetRoom.GetRoomUserManager().GetRoomUserByName(Convert.ToString(Params[1]));
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))

            //if ((TargetRoomUser.X == TargetRoomUser1.X - 1) || (TargetRoomUser.X == TargetRoomUser1.X + 1) || (TargetRoomUser.Y == TargetRoomUser1.Y - 1) || (TargetRoomUser.Y == TargetRoomUser1.Y + 1))
            if (!((Math.Abs((TargetRoomUser.X - TargetRoomUser1.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - TargetRoomUser1.Y)) >= 2)))