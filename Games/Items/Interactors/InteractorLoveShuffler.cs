namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorLoveShuffler : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item) => item.ExtraData = "-1";

    public override void OnRemove(GameClient session, Item item) => item.ExtraData = "-1";

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights || !(item.ExtraData != "0"))
        {
            return;
        }

        item.ExtraData = "0";
        item.UpdateState(false, true);
        item.ReqUpdate(10);
    }

    public override void OnTick(Item item)
    {
        if (item.ExtraData == "0")
        {
            item.ExtraData = WibboEnvironment.GetRandomNumber(1, 4).ToString();
            item.ReqUpdate(20);
        }
        else if (item.ExtraData != "-1")
        {
            item.ExtraData = "-1";
        }

        item.UpdateState(false, true);
    }
}
