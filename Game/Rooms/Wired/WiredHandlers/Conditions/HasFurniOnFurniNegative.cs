using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Pathfinding;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class HasFurniOnFurniNegative : WiredConditionBase, IWiredCondition, IWired
    {
        public HasFurniOnFurniNegative(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_HAS_STACKED_FURNIS)
        {
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            bool requireAll = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

            if (this.Items.Count == 0)
                return true;

            foreach (Item roomItem in this.Items)
            {
                foreach (ThreeDCoord coord in roomItem.GetAffectedTiles.Values)
                {
                    if (this.RoomInstance.GetGameMap().Model.SqFloorHeight[coord.X, coord.Y] + this.RoomInstance.GetGameMap().ItemHeightMap[coord.X, coord.Y] > roomItem.TotalHeight)
                    {
                        if (requireAll)
                            return false;
                    }
                    else
                    {
                        if (!requireAll)
                            return true;
                    }
                }
            }

            return requireAll;
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems(inDatabase);

            if(this.IntParams.Count == 0)
                this.IntParams.Add(1);
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int requireAll = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, requireAll.ToString(), false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int requireAll))
                this.IntParams.Add(requireAll);

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
