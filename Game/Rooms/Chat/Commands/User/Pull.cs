using Butterfly.Game.GameClients;
using System;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
            {
                return;
            }

            if (!Room.PushPullAllowed)
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByName(Convert.ToString(Params[1]));
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))
                {
                    UserRoom.RotBody--;
                }

                if (UserRoom.RotBody == 0)
                {
                    TargetUser.MoveTo(UserRoom.X, UserRoom.Y - 1);
                }
                else if (UserRoom.RotBody == 2)
                {
                    TargetUser.MoveTo(UserRoom.X + 1, UserRoom.Y);
                }
                else if (UserRoom.RotBody == 4)
                {
                    TargetUser.MoveTo(UserRoom.X, UserRoom.Y + 1);
                }
                else if (UserRoom.RotBody == 6)
                {
                    TargetUser.MoveTo(UserRoom.X - 1, UserRoom.Y);
                }