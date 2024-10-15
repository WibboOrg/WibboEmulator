namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorWired : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item)
    {
    }

    public override void OnRemove(GameClient Session, Item item)
    {
    }

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (Session == null || item == null || !userHasRights)
        {
            return;
        }

        item.WiredHandler?.OnTrigger(Session);

        item.ExtraData = "1";
        item.UpdateState();
        item.ReqUpdate(4);
    }

    public override void OnTick(Item item)
    {
        item.ExtraData = "0";
        item.UpdateState();
    }
}
