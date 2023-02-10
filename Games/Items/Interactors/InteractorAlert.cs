namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorAlert : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item) => item.ExtraData = "0";

    public override void OnRemove(GameClient session, Item item) => item.ExtraData = "0";

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights || !(item.ExtraData == "0"))
        {
            return;
        }

        item.ExtraData = "1";
        item.UpdateState(false);
        item.ReqUpdate(4);
    }

    public override void OnTick(Item item)
    {
        if (!(item.ExtraData == "1"))
        {
            return;
        }

        item.ExtraData = "0";
        item.UpdateState(false);
    }
}
