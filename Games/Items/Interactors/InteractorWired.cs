namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorWired : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || item == null || !userHasRights)
        {
            return;
        }

        if (item.WiredHandler != null)
        {
            item.WiredHandler.OnTrigger(session);
        }

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
