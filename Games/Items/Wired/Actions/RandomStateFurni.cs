namespace WibboEmulator.Games.Items.Wired.Actions;

using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class RandomStateFurni : WiredActionBase, IWired, IWiredEffect
{
    public RandomStateFurni(Item item, Room room) : base(item, room, (int)WiredActionType.FLEE) => this.DefaultIntParams(0);

    public override bool OnCycle(RoomUser user, Item item)
    {
        var isReverse = this.GetIntParam(0) == 1;

        foreach (var roomItem in this.Items.ToList())
        {
            if (roomItem != null)
            {
                if (roomItem.ItemData.Modes > 1)
                {
                    if (int.TryParse(roomItem.ExtraData, out var stateItem))
                    {
                        var newState = WibboEnvironment.GetRandomNumber(0, roomItem.ItemData.Modes);

                        if (newState != stateItem)
                        {
                            roomItem.ExtraData = newState.ToString();
                            roomItem.UpdateState();
                        }
                    }
                }
            }
        }

        return false;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.LoadStuffIds(wiredTriggersItem);
    }
}
