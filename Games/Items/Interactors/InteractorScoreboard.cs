namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorScoreboard : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item)
    {
    }

    public override void OnRemove(GameClient Session, Item item)
    {
    }

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights)
        {
            return;
        }

        var num = 0;
        if (!string.IsNullOrEmpty(item.ExtraData))
        {

            _ = int.TryParse(item.ExtraData, out num);
        }

        if (request == 1)
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
        else if (request == 2)
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
        else if (request == 3)
        {
            num = 0;
        }

        item.ExtraData = num.ToString();
        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
        if (string.IsNullOrEmpty(item.ExtraData))
        {
            return;
        }

        _ = int.TryParse(item.ExtraData, out var num);

        if (num > 0)
        {
            if (item.InteractionCountHelper == 1)
            {
                var score = num - 1;
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
