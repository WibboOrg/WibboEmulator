using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms.Games;
using System;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Push : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            RoomUser TargetRoomUser;
            RoomUser TargetRoomUser1;

            Room TargetRoom = Session.GetHabbo().CurrentRoom;

            if (TargetRoom == null)
            {
                return;
            }

            if (!TargetRoom.PushPullAllowed)
            {
                return;
            }

            TargetRoomUser1 = TargetRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (TargetRoomUser1 == null)
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            TargetRoomUser = TargetRoom.GetRoomUserManager().GetRoomUserByName(Convert.ToString(Params[1]));

            if (TargetRoomUser == null)
            {
                TargetRoomUser1.SendWhisperChat(Convert.ToString(Params[1]) + " n'est plus ici.");
                return;
            }

            if (TargetRoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                return;
            }

            RoomUser TUS = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (TUS == null)
                return;

            if (!(Math.Abs(TargetRoomUser.X - TUS.X) >= 2) || (Math.Abs(TargetRoomUser.Y - TUS.Y) >=2))
            {
                if (TargetRoomUser.SetX - 1 == Room.GetGameMap().Model.DoorX)
                    return;

            }

            //if ((TargetRoomUser.X == TargetRoomUser1.X - 1) || (TargetRoomUser.X == TargetRoomUser1.X + 1) || (TargetRoomUser.Y == TargetRoomUser1.Y - 1) || (TargetRoomUser.Y == TargetRoomUser1.Y + 1))
            if (!((Math.Abs((TargetRoomUser.X - TargetRoomUser1.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - TargetRoomUser1.Y)) >= 2)))
            {
                if (TargetRoomUser1.RotBody == 4)
                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1); }

                if (TargetRoomUser1.RotBody == 0)
                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1); }

                if (TargetRoomUser1.RotBody == 6)
                { TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y); }

                if (TargetRoomUser1.RotBody == 2)
                { TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y); }

                if (TargetRoomUser1.RotBody == 3)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);
                }

                if (TargetRoomUser1.RotBody == 1)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);
                }

                if (TargetRoomUser1.RotBody == 7)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);
                }

                if (TargetRoomUser1.RotBody == 5)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);
                }

                TargetRoomUser1.OnChat("*pousse " + Params[1] + "*", 0, false);
            }
            else
            {
                TargetRoomUser1.SendWhisperChat(Params[1] + " est trop loin de vous.");
            }

        }
    }
}
