using WibboEmulator.Game.Clients;

namespace WibboEmulator.Game.Items.Interactors
{
    public class InteractorScoreboard : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
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
                if (num > 0)
                {
                    num -= 1;
                }
                else
                {
                    num = 99;
                }
            }
            else if (Request == 2)
            {
                if (num < 99)
                {
                    num += 1;
                }
                else
                {
                    num = 0;
                }
            }
            else if (Request == 3)
            {
                num = 0;
            }

            Item.ExtraData = num.ToString();
            Item.UpdateState();
        }

        public override void OnTick(Item item)
        {
            if (string.IsNullOrEmpty(item.ExtraData))
            {
                return;
            }

            int.TryParse(item.ExtraData, out int num);

            if (num > 0)
            {
                if (item.InteractionCountHelper == 1)
                {
                    int score = num - 1;
                    item.InteractionCountHelper = 0;
                    item.ExtraData = score.ToString();
                    item.UpdateState();
                }
                else
                {
                    item.InteractionCountHelper++;
                }

                item.UpdateCounter = 1;
            }
            else
            {
                item.UpdateCounter = 0;
            }
        }
    }
}
