namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorHabboWheel : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item) => item.ExtraData = "0";

    public override void OnRemove(GameClient Session, Item item) => item.ExtraData = "0";

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights || !(item.ExtraData != "-1"))
        {
            return;
        }

        item.ExtraData = "-1";
        item.UpdateState();

        item.ReqUpdate(10);
    }

    public override void OnTick(Item item)
    {
        item.ExtraData = WibboEnvironment.GetRandomNumber(1, 10).ToString();
        item.UpdateState();
    }
}
