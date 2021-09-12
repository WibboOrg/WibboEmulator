using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class CloseDice : IChatCommand
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
            List<Item> userBooth = Room.GetRoomItemHandler().GetFloor.Where(x => x != null && Gamemap.TilesTouching(
                x.GetX, x.GetY, UserRoom.X, UserRoom.Y) && x.Data.InteractionType == Items.InteractionType.DICE).ToList();

            if (userBooth == null)
            {
                return;
            }

            userBooth.ForEach(x =>
            {
                x.ExtraData = "0";
                x.UpdateState();
            });
        }
    }
}
