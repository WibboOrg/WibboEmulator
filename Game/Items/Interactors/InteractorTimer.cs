using Butterfly.Game.Clients;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorTimer : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
            {
                return;
            }

            int time = 0;
            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                 int.TryParse(Item.ExtraData, out time);
            }

            if (Request == 2)
            {
                if (Item.PendingReset && time > 0)
                {
                    Item.ChronoStarter = false;
                    Item.PendingReset = false;
                }
                else
                {
                    if (time == 0 || time == 30 || time == 60 || time == 120 || time == 180 || time == 300 || time == 600)
                    {
                        if (time == 0)
                        {
                            time = 30;
                        }
                        else if (time == 30)
                        {
                            time = 60;
                        }
                        else if (time == 60)
                        {
                            time = 120;
                        }
                        else if (time == 120)
                        {
                            time = 180;
                        }
                        else if (time == 180)
                        {
                            time = 300;
                        }
                        else if (time == 300)
                        {
                            time = 600;
                        }
                        else if (time == 600)
                        {
                            time = 0;
                        }
                    }
                    else
                    {
                        time = 0;
                    }
                }
            }
            else if ((Request == 0 || Request == 1) && time != 0 && !Item.ChronoStarter)
            {
                Item.ReqUpdate(1);
                Item.GetRoom().GetGameManager().StartGame();
                Item.ChronoStarter = true;
                Item.PendingReset = true;
            }

            Item.ExtraData = time.ToString();
            Item.UpdateState();
        }

        public override void OnTick(Item item)
        {
            if (string.IsNullOrEmpty(item.ExtraData))
            {
                return;
            }

            int time;
            if (!int.TryParse(item.ExtraData, out time))
            {
                return;
            }

            if (!item.ChronoStarter)
            {
                return;
            }

            if (time > 0)
            {
                if (item.InteractionCountHelper == 1)
                {
                    time--;

                    item.InteractionCountHelper = 0;
                    item.ExtraData = time.ToString();
                    item.UpdateState();
                }
                else
                {
                    item.InteractionCountHelper++;
                }

                item.UpdateCounter = 1;
                return;
            }
            else
            {
                item.ChronoStarter = false;
                item.GetRoom().GetGameManager().StopGame();
                return;
            }
        }
    }
}
