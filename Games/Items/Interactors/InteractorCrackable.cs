namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorCrackable : FurniInteractor
{
    private readonly int _modes;

    public InteractorCrackable(int modes)
    {
        this._modes = modes - 1;
        if (this._modes >= 0)
        {
            return;
        }

        this._modes = 0;
    }

    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights || this._modes == 0)
        {
            return;
        }

        _ = int.TryParse(item.ExtraData, out var numMode);

        numMode++;

        if (numMode > this._modes)
        {
            numMode = 0;
        }

        item.ExtraData = numMode.ToString();
        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
