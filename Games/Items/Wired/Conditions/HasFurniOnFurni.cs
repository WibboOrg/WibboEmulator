namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class HasFurniOnFurni : WiredConditionBase, IWiredCondition, IWired
{
    public HasFurniOnFurni(Item item, Room room) : base(item, room, (int)WiredConditionType.HAS_STACKED_FURNIS) => this.IntParams.Add(0);

    public bool AllowsExecution(RoomUser user, Item item)
    {
        var requireAll = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

        if (this.Items.Count == 0)
        {
            return false;
        }

        foreach (var roomItem in this.Items.ToList())
        {
            foreach (var coord in roomItem.GetAffectedTiles)
            {
                if (this.RoomInstance.GetGameMap().Model.SqFloorHeight[coord.X, coord.Y] + this.RoomInstance.GetGameMap().ItemHeightMap[coord.X, coord.Y] > roomItem.TotalHeight)
                {
                    if (!requireAll)
                    {
                        return true;
                    }
                }
                else
                {
                    if (requireAll)
                    {
                        return false;
                    }
                }
            }
        }

        return requireAll;
    }

    public void SaveToDatabase(IQueryAdapter dbClient)
    {
        var requireAll = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
        WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, requireAll.ToString(), false, this.Items);
    }

    public void LoadFromDatabase(DataRow row)
    {
        this.IntParams.Clear();

        if (int.TryParse(row["trigger_data"].ToString(), out var requireAll))
        {
            this.IntParams.Add(requireAll);
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
