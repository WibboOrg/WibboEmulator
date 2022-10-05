namespace WibboEmulator.Games.Items.Wired.Actions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ToggleItemState : WiredActionBase, IWired, IWiredEffect
{
    public ToggleItemState(Item item, Room room) : base(item, room, (int)WiredActionType.TOGGLE_FURNI_STATE) => this.IntParams.Add(0);

    public override bool OnCycle(RoomUser user, Item item)
    {
        var isReverse = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        foreach (var roomItem in this.Items.ToList())
        {
            if (roomItem != null)
            {
                if (user != null && user.GetClient() != null)
                {
                    roomItem.Interactor.OnTrigger(user.GetClient(), roomItem, 0, true, isReverse);
                }
                else
                {
                    roomItem.Interactor.OnTrigger(null, roomItem, 0, true, isReverse);
                }

            }
        }

        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var reverse = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;

        WiredUtillity.SaveTriggerItem(dbClient, this.Id, reverse.ToString(), string.Empty, false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["delay"].ToString(), out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data"].ToString(), out delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(row["trigger_data_2"].ToString(), out var reverse))
        {
            this.IntParams.Add(reverse);
        }

        var triggerItems = row["triggers_item"].ToString();

        if (triggerItems is null or "")
        {
            return;
        }

        foreach (var itemId in triggerItems.Split(';'))
        {
            if (!int.TryParse(itemId, out var id))
            {
                continue;
            }

            if (!this.StuffIds.Contains(id))
            {
                this.StuffIds.Add(id);
            }
        }
    }
}
