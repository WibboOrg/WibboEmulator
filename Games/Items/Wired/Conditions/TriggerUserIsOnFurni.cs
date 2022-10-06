namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using System.Drawing;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class TriggerUserIsOnFurni : WiredConditionBase, IWiredCondition, IWired
{
    public TriggerUserIsOnFurni(Item item, Room room) : base(item, room, (int)WiredConditionType.TRIGGERER_IS_ON_FURNI)
    {
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (user == null)
        {
            return false;
        }

        Point coord;

        foreach (var roomItem in this.Items.ToList())
        {
            foreach (var coor in roomItem.GetAffectedTiles.Values)
            {
                coord = new Point(coor.X, coor.Y);
                if (coord == user.Coordinate)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);

    public void LoadFromDatabase(DataRow row)
    {
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
