using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class HandItem : IChatCommand
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
            if (Params.Length != 2)
            {
                return;
            }

            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            int handitemid = -1;
            int.TryParse(Params[1], out handitemid);
            if (handitemid < 0)
            {
                return;
            }

            UserRoom.CarryItem(handitemid);

        }
    }
}
