namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorCrackable : FurniInteractor
{
    private readonly int Modes;

    public InteractorCrackable(int Modes)
    {
        this.Modes = Modes - 1;
        if (this.Modes >= 0)
        {
            return;
        }

        this.Modes = 0;
    }

    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights || this.Modes == 0)
        {
            return;
        }

        int.TryParse(item.ExtraData, out var NumMode);

        NumMode++;

        if (NumMode > this.Modes)
        {
            NumMode = 0;
        }

        item.ExtraData = NumMode.ToString();
        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
