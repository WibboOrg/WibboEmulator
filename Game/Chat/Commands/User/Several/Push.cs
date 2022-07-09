using Wibbo.Game.Clients;
using Wibbo.Game.Rooms.Games;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Push : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            RoomUser TargetRoomUser;
            RoomUser TargetRoomUser1;

            Room TargetRoom = Session.GetUser().CurrentRoom;

            if (TargetRoom == null)
            {
                return;
            }

            if (!TargetRoom.PushPullAllowed)
            {
                return;
            }

            TargetRoomUser1 = TargetRoom.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
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

            if (TargetRoomUser.GetClient().GetUser().Id == Session.GetUser().Id)
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetUser().PremiumProtect && !Session.GetUser().HasFuse("fuse_mod"))
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                return;
            }

            RoomUser TUS = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
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
