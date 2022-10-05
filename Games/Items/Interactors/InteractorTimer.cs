namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorTimer : FurniInteractor
{
    private bool _pendingReset = false;
    private bool _chronoStarter = false;

    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item) => item.ExtraData = "0";

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights)
        {
            return;
        }

        var time = 0;
        if (!string.IsNullOrEmpty(item.ExtraData))
        {
            int.TryParse(item.ExtraData, out time);
        }

        if (request == 2)
        {
            if (this._pendingReset && time > 0)
            {
                this._chronoStarter = false;
                this._pendingReset = false;
            }
            else
            {
                if (time is 0 or 30 or 60 or 120 or 180 or 300 or 600)
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
        else if ((request == 0 || request == 1) && time != 0 && !this._chronoStarter)
        {
            item.ReqUpdate(1);
            item.GetRoom().GetGameManager().StartGame();
            this._chronoStarter = true;
            this._pendingReset = true;
        }

        item.ExtraData = time.ToString();
        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
        if (item == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(item.ExtraData))
        {
            return;
        }

        if (!int.TryParse(item.ExtraData, out var time))
        {
            return;
        }

        if (!this._chronoStarter)
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
            this._chronoStarter = false;
            item.GetRoom().GetGameManager().StopGame();
            return;
        }
    }
}
