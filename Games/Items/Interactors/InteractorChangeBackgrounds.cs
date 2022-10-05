namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;

public class InteractorChangeBackgrounds : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || session.GetUser() == null || item == null || !userHasRights || request != 0)
        {
            return;
        }

        if (item.ExtraData.StartsWith("on"))
        {
            item.ExtraData = item.ExtraData.Replace("on", "off");
        }
        else if (item.ExtraData.StartsWith("off"))
        {
            item.ExtraData = item.ExtraData.Replace("off", "on");
        }

        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
