using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Rooms.PathFinding;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;

namespace Wibbo.Game.Items.Wired.Conditions
{
    public class HasFurniOnFurni : WiredConditionBase, IWiredCondition, IWired
    {
        public HasFurniOnFurni(Item item, Room room) : base(item, room, (int)WiredConditionType.HAS_STACKED_FURNIS)
        {
            this.IntParams.Add(0);
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            bool requireAll = ((this.IntParams.Count > 0) ? this.IntParams[0] : 0) == 1;

            if (this.Items.Count == 0) 
                return false;

            foreach (Item roomItem in this.Items)
            {
                foreach (Coord coord in roomItem.GetAffectedTiles.Values)
                {
                    if (this.RoomInstance.GetGameMap().Model.SqFloorHeight[coord.X, coord.Y] + this.RoomInstance.GetGameMap().ItemHeightMap[coord.X, coord.Y] > roomItem.TotalHeight)
                    {
                        if (!requireAll)
                            return true;
                    }
                    else
                    {
                        if (requireAll)
                            return false;
                    }
                }
            }

            return requireAll;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            int requireAll = (this.IntParams.Count > 0) ? this.IntParams[0] : 0;
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, requireAll.ToString(), false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

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
