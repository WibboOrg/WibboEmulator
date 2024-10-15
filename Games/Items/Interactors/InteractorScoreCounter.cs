namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Games.Teams;

public class InteractorScoreCounter : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item)
    {
        if (item.Team == TeamType.None)
        {
            return;
        }

        item.ExtraData = item.Room.GameManager.Points[(int)item.Team].ToString();
        item.UpdateState(false);
    }

    public override void OnRemove(GameClient Session, Item item)
    {
    }

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
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
        item.UpdateState(false);
    }

    public override void OnTick(Item item)
    {
    }
}
