namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorGate : FurniInteractor
{
    public InteractorGate()
    {
    }

    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {

        if (!userHasRights)
        {
            return;
        }

        _ = int.TryParse(item.ExtraData, out var currentMode);

        int newMode;
        if (currentMode == 0)
        {
            newMode = 1;
        }
        else
        {
            newMode = 0;
        }

        item.ExtraData = newMode.ToString();
        item.UpdateState();
        item.        Room?.GameMap.UpdateMapForItem(item);
    }

    public override void OnTick(Item item)
    {
    }
}
