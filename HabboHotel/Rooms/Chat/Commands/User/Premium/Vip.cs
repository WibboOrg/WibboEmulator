using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Vip : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            int CurrentEnable = UserRoom.CurrentEffect;
            if (CurrentEnable == 28 || CurrentEnable == 29 || CurrentEnable == 30 || CurrentEnable == 37 || CurrentEnable == 184 || CurrentEnable == 77 || CurrentEnable == 103
                || CurrentEnable == 40 || CurrentEnable == 41 || CurrentEnable == 42 || CurrentEnable == 43
                || CurrentEnable == 49 || CurrentEnable == 50 || CurrentEnable == 51 || CurrentEnable == 52
                || CurrentEnable == 33 || CurrentEnable == 34 || CurrentEnable == 35 || CurrentEnable == 36)
            {
                return;
            }

            if (UserRoom.CurrentEffect == 569)
            {
                UserRoom.ApplyEffect(0);
            }
            else
            {
                UserRoom.ApplyEffect(569);
            }
        }
    }
}
