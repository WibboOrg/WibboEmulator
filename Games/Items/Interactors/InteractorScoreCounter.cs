using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Games;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorScoreCounter : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            if (Item.Team == TeamType.NONE)
            {
                return;
            }

            Item.ExtraData = Item.GetRoom().GetGameManager().Points[(int)Item.Team].ToString();
            Item.UpdateState(false, true);
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!UserHasRights)
            {
                return;
            }

            int num = 0;
            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                int.TryParse(Item.ExtraData, out num);
            }

            if (Request == 1)
            {
                num++;
            }
            else if (Request == 2)
            {
                num--;
            }
            else if (Request == 3)
            {
                num = 0;
            }

            Item.ExtraData = num.ToString();
            Item.UpdateState(false, true);
        }

        public override void OnTick(Item item)
        {
        }
    }
}
