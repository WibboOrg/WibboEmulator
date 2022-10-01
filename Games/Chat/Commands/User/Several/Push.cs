using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms.Games;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Push : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
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

            RoomUser TargetRoomUser = Room.GetRoomUserManager().GetRoomUserByName(Convert.ToString(Params[1]));

            if (TargetRoomUser == null)
            {
                UserRoom.SendWhisperChat(Convert.ToString(Params[1]) + " n'est plus ici.");
                return;
            }

            if (TargetRoomUser.GetClient().GetUser().Id == Session.GetUser().Id)
            {
                return;
            }

            if (TargetRoomUser.GetClient().GetUser().PremiumProtect && !Session.GetUser().HasPermission("perm_mod"))
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                return;
            }

            if (!(Math.Abs(TargetRoomUser.X - UserRoom.X) >= 2) || (Math.Abs(TargetRoomUser.Y - UserRoom.Y) >=2))
            {
                if (TargetRoomUser.SetX - 1 == Room.GetGameMap().Model.DoorX)
                    return;
            }

            if (!((Math.Abs((TargetRoomUser.X - UserRoom.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - UserRoom.Y)) >= 2)))
            {
                if (UserRoom.RotBody == 4)
                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1); }

                if (UserRoom.RotBody == 0)
                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1); }

                if (UserRoom.RotBody == 6)
                { TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y); }

                if (UserRoom.RotBody == 2)
                { TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y); }

                if (UserRoom.RotBody == 3)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);
                }

                if (UserRoom.RotBody == 1)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);
                }

                if (UserRoom.RotBody == 7)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);
                }

                if (UserRoom.RotBody == 5)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);
                }

                UserRoom.OnChat("*pousse " + Params[1] + "*", 0, false);
            }
            else
            {
                UserRoom.SendWhisperChat(Params[1] + " est trop loin de vous.");
            }
        }
    }
}
