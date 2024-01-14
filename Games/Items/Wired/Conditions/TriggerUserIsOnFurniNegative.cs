namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class TriggerUserIsOnFurniNegative : WiredConditionBase, IWiredCondition, IWired
{
    public TriggerUserIsOnFurniNegative(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_ACTOR_ON_FURNI)
    {
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (user == null)
        {
            return false;
        }

        foreach (var roomItem in this.Items.ToList())
        {
            foreach (var coord in roomItem.GetAffectedTiles)
            {
                if (coord == user.Coordinate)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.LoadStuffIds(wiredTriggersItem);
}
