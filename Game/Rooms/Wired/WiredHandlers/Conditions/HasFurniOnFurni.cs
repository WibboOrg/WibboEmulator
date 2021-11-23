using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Pathfinding;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class HasFurniOnFurni : WiredConditionBase, IWiredCondition, IWired
    {
        public HasFurniOnFurni(Item item, Room room) : base(item, room, (int)WiredConditionType.HAS_STACKED_FURNIS)
        {
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            foreach (Item roomItem in this.Items)
            {
                foreach (ThreeDCoord coord in roomItem.GetAffectedTiles.Values)
                {
                    if (!this.RoomInstance.GetGameMap().ValidTile(coord.X, coord.Y))
                    {
                        return false;
                    }

                    if (this.RoomInstance.GetGameMap().Model.SqFloorHeight[coord.X, coord.Y] + this.RoomInstance.GetGameMap().ItemHeightMap[coord.X, coord.Y] > roomItem.TotalHeight)
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
