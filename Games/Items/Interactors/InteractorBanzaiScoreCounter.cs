namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Games;

public class InteractorBanzaiScoreCounter : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        if (item.Team == TeamType.NONE)
        {
            return;
        }

        item.ExtraData = item.GetRoom().GetGameManager().Points[(int)item.Team].ToString();
        item.UpdateState(false, true);
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (!userHasRights)
        {
            return;
        }

        item.GetRoom().GetGameManager().Points[(int)item.Team] = 0;
        item.ExtraData = "0";
        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
