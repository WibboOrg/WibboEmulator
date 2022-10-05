using System.Data;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired.Conditions
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

            foreach (Item roomItem in this.Items.ToList())
            {
                if (roomItem.BaseItem == triggerItem.BaseItem && roomItem.ExtraData == triggerItem.ExtraData)
                {
                    return true;
                }
            }

            return false;
        }

        public void SaveToDatabase(IQueryAdapter dbClient) => WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, string.Empty, false, this.Items);

        public void LoadFromDatabase(DataRow row)
        {
            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == null || triggerItems == "")
            {
                return;
            }

            foreach (string itemId in triggerItems.Split(';'))
            {
                if (!int.TryParse(itemId, out int id))
                    continue;

                if (!this.StuffIds.Contains(id))
                    this.StuffIds.Add(id);
            }
        }
    }
}
