using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorDice : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Session == null)
            {
                return;
            }

            RoomUser roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

            if (roomUser == null)
            {
                return;
            }

            if (Gamemap.TilesTouching(Item.X, Item.Y, roomUser.X, roomUser.Y))
            {
                if (!(Item.ExtraData != "-1"))
                {
                    return;
                }

                if (Request == -1)
                {
                    Item.ExtraData = "0";
                    Item.UpdateState();

                    if (roomUser.DiceCounterAmount > 0 && !roomUser.InGame)
                    {
                        roomUser.DiceCounterAmount = 0;
                        roomUser.DiceCounter = 0;
                        roomUser.OnChat($"Dée: remise à 0 ({roomUser.GetUsername()})", 34); // déplacer
                    }
                }
                else
                {
                    Item.ExtraData = "-1";
                    Item.UpdateState(false, true);
                    Item.ReqUpdate(4);

                    Item.InteractingUser = roomUser.UserId;
                }
            }
            else
            {
                roomUser.MoveTo(Item.SquareInFront);
            }
        }

        public override void OnTick(Item item)
        {
            int numberDice = WibboEnvironment.GetRandomNumber(1, 6);

            item.ExtraData = numberDice.ToString();
            item.UpdateState();

            RoomUser user = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
            if (user != null)
            {
                if (!user.InGame)
                {
                    user.DiceCounterAmount += numberDice;
                    user.DiceCounter++;
                    user.OnChat($"Dée {user.DiceCounter}: +{numberDice} = {user.DiceCounterAmount} ({user.GetUsername()})", 34);
                }
            }
         }
    }
}
