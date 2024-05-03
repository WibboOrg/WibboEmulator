namespace WibboEmulator.Games.Items.Wired.Actions;

using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class ToggleItemState : WiredActionBase, IWired, IWiredEffect
{
    public ToggleItemState(Item item, Room room) : base(item, room, (int)WiredActionType.TOGGLE_FURNI_STATE) => this.DefaultIntParams(0);

    public override bool OnCycle(RoomUser user, Item item)
    {
        var isReverse = this.GetIntParam(0) == 1;

        foreach (var roomItem in this.Items.ToList())
        {
            if (roomItem != null)
            {
                if (user != null && user.Client != null)
                {
                    roomItem.Interactor.OnTrigger(user.Client, roomItem, 0, true, isReverse);
                }
                else
                {
                    roomItem.Interactor.OnTrigger(null, roomItem, 0, true, isReverse);
                }
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var reverse = this.GetIntParam(0);

        WiredUtillity.SaveInDatabase(dbClient, this.Id, reverse.ToString(), string.Empty, false, this.Items, this.Delay);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        if (int.TryParse(wiredTriggerData, out var delay))
        {
            this.Delay = delay;
        }

        if (int.TryParse(wiredTriggerData2, out var reverse))
        {
            this.SetIntParam(0, reverse);
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
