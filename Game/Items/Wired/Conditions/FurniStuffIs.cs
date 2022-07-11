using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Items.Wired.Conditions
{
    public class FurniStuffIs : WiredConditionBase, IWiredCondition, IWired
    {
        public FurniStuffIs(Item item, Room room) : base(item, room, (int)WiredConditionType.STUFF_TYPE_MATCHES)
        {
        }

        public bool AllowsExecution(RoomUser user, Item triggerItem)
        {
            if (triggerItem == null)
            {
                return false;
            }

            foreach (Item roomItem in this.Items)
            {
                if (roomItem.BaseItem == triggerItem.BaseItem && roomItem.ExtraData == triggerItem.ExtraData)
                {
                    return true;
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
