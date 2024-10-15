namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public abstract class FurniInteractor
{
    public abstract void OnPlace(GameClient Session, Item item);

    public abstract void OnRemove(GameClient Session, Item item);

    public abstract void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse);

    public abstract void OnTick(Item item);
}
