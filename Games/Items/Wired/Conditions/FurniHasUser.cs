namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class FurniHasUser : WiredConditionBase, IWiredCondition, IWired
{
    public FurniHasUser(Item item, Room room) : base(item, room, (int)WiredConditionType.FURNIS_HAVE_AVATARS)
    {
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        foreach (var itemList in this.Items.ToList())
        {
            foreach (var coord in itemList.GetAffectedTiles)
            {
                if (this.RoomInstance.GetGameMap().GetRoomUsers(coord).Count == 0)
                {
                    return false;
                }
            }
        }

        return true;
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
