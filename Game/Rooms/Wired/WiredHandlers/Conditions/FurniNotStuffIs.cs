using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class FurniNotStuffIs : WiredConditionBase, IWiredCondition, IWired
    {
        public FurniNotStuffIs(Item item, Room room) : base(item, room, (int)WiredConditionType.NOT_FURNI_IS_OF_TYPE)
        {
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (TriggerItem == null)
            {
                return false;
            }

            foreach (Item roomItem in this.Items)
            {
                if (roomItem.BaseItem == TriggerItem.BaseItem && roomItem.ExtraData == TriggerItem.ExtraData)
                {
                    return false;
                }
            }
            return true;
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
