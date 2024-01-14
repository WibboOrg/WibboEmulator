namespace WibboEmulator.Games.Items.Wired.Conditions;
using System.Drawing;
using System.Data;
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
            foreach (var coor in roomItem.GetAffectedTiles)
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

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay) => this.LoadStuffIds(wiredTriggersItem);
}
