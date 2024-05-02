namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class HasFurniOnFurniNegative : WiredConditionBase, IWiredCondition, IWired
{
    public HasFurniOnFurniNegative(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_HAS_STACKED_FURNIS) => this.DefaultIntParams(new int[] { 1 });

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var requireAll = this.GetIntParam(0) == 1;

        if (this.Items.Count == 0)
        {
            return true;
        }

        foreach (var roomItem in this.Items.ToList())
        {
            foreach (var coord in roomItem.GetAffectedTiles)
            {
                if (this.Room.GameMap.Model.SqFloorHeight[coord.X, coord.Y] + this.Room.GameMap.ItemHeightMap[coord.X, coord.Y] > roomItem.TotalHeight)
                {
                    if (requireAll)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!requireAll)
                    {
                        return true;
                    }
                }
            }
        }

        return requireAll;
    }

    public void SaveToDatabase(IDbConnection dbClient)
    {
        var requireAll = this.GetIntParam(0);
        WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, requireAll.ToString(), false, this.Items);
    }

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        if (int.TryParse(wiredTriggerData, out var requireAll))
        {
            this.SetIntParam(0, requireAll);
        }

        this.LoadStuffIds(wiredTriggersItem);
    }
}
