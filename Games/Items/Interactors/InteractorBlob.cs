namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorBlob : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item) => item.ExtraData = "1";

    public override void OnRemove(GameClient Session, Item item) => item.ExtraData = "1";

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights || item.ExtraData == "0")
        {
            return;
        }

        item.ExtraData = "0";
        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
