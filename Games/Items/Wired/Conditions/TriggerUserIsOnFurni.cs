using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.PathFinding;
using WibboEmulator.Games.Items.Wired.Interfaces;
using System.Data;
using System.Drawing;

namespace WibboEmulator.Games.Items.Wired.Conditions
{
    public class TriggerUserIsOnFurni : WiredConditionBase, IWiredCondition, IWired
    {
        public TriggerUserIsOnFurni(Item item, Room room) : base(item, room, (int)WiredConditionType.TRIGGERER_IS_ON_FURNI)
        {
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (user == null)
            {
                return false;
            }

            Point coord;

            foreach (Item roomItem in this.Items)
            {
                foreach (Coord coor in roomItem.GetAffectedTiles.Values)
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

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
            {
                return;
            }

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if(!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
