namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Games.Teams;

public class InteractorBanzaiScoreCounter : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        if (item.Team == TeamType.None)
        {
            return;
        }

        item.ExtraData = item.GetRoom().GameManager.Points[(int)item.Team].ToString();
        item.UpdateState(false);
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

        item.GetRoom().
        GameManager.Points[(int)item.Team] = 0;
        item.ExtraData = "0";
        item.UpdateState();
    }

    public override void OnTick(Item item)
    {
    }
}
