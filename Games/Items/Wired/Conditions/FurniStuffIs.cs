namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class FurniStuffIs : WiredConditionBase, IWiredCondition, IWired
{
    public FurniStuffIs(Item item, Room room) : base(item, room, (int)WiredConditionType.STUFF_TYPE_MATCHES)
    {
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        if (item == null)
        {
            return false;
        }

        foreach (var roomItem in this.Items.ToList())
        {
            if (roomItem.BaseItem == item.BaseItem && roomItem.ExtraData == item.ExtraData)
            {
                return true;
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
