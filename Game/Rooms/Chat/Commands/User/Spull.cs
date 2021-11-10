using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
            {
                return;
            }

            RoomUser roomuser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            RoomUser TargetUser = room.GetRoomUserManager().GetRoomUserByName(Params[1]);
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                roomuser.RotBody--;
            }

            if (roomuser.RotBody == 0)
            {
                TargetUser.MoveTo(roomuser.X, roomuser.Y - 1);
            }
            else if (roomuser.RotBody == 2)
            {
                TargetUser.MoveTo(roomuser.X + 1, roomuser.Y);
            }
            else if (roomuser.RotBody == 4)
            {
                TargetUser.MoveTo(roomuser.X, roomuser.Y + 1);
            }
            else if (roomuser.RotBody == 6)
            {
                TargetUser.MoveTo(roomuser.X - 1, roomuser.Y);
            }