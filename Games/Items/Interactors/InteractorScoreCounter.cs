namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Games.Teams;

public class InteractorScoreCounter : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        if (item.Team == TeamType.None)
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

        var num = 0;
        if (!string.IsNullOrEmpty(item.ExtraData))
        {
            _ = int.TryParse(item.ExtraData, out num);
        }

        if (request == 1)
        {
            num++;
        }
        else if (request == 2)
        {
            num--;
        }
        else if (request == 3)
        {
            num = 0;
        }

        item.ExtraData = num.ToString();
        item.UpdateState(false, true);
    }

    public override void OnTick(Item item)
    {
    }
}
