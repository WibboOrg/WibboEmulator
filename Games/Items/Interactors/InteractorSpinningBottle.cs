namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorSpinningBottle : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        item.ExtraData = "0";
        item.UpdateState();
    }

    public override void OnRemove(GameClient session, Item item) => item.ExtraData = "0";

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!(item.ExtraData != "-1"))
        {
            return;
        }

        item.ExtraData = "-1";
        item.UpdateState(false);
        item.ReqUpdate(3);
    }

    public override void OnTick(Item item)
    {
        item.ExtraData = WibboEnvironment.GetRandomNumber(0, 7).ToString();
        item.UpdateState();
    }
}
