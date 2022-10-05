namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public abstract class FurniInteractor
{
    public abstract void OnPlace(GameClient session, Item item);

    public abstract void OnRemove(GameClient session, Item item);

    public abstract void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse);

    public abstract void OnTick(Item item);
}
