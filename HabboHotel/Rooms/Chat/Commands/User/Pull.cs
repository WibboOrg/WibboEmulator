using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Pull : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
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

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByName(Convert.ToString(Params[1]));            if (TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (TargetUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));                return;            }            if (Math.Abs(UserRoom.X - TargetUser.X) < 3 && Math.Abs(UserRoom.Y - TargetUser.Y) < 3)            {                UserRoom.OnChat("*Tire " + Params[1] + "*", 0, false);                if (UserRoom.RotBody % 2 != 0)
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
                }            }            else            {                UserRoom.SendWhisperChat(Params[1] + " est trop loin de vous.");                return;            }        }    }}