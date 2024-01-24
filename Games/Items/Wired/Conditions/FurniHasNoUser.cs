namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public class FurniHasNoUser : WiredConditionBase, IWiredCondition, IWired
{
    public FurniHasNoUser(Item item, Room room) : base(item, room, (int)WiredConditionType.FURNI_NOT_HAVE_HABBO)
    {
    }

    public bool AllowsExecution(RoomUser user, Item item)
    {
        foreach (var itemList in this.Items.ToList())
        {
            foreach (var coord in itemList.GetAffectedTiles)
            {
                if (this.RoomInstance.GameMap.GetRoomUsers(coord).Count != 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.LoadStuffIds(wiredTriggersItem);
}
