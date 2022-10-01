using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;
using System.Drawing;

namespace WibboEmulator.Game.Items.Wired.Conditions
{
    public class FurniHasUser : WiredConditionBase, IWiredCondition, IWired
    {
        public FurniHasUser(Item item, Room room) : base(item, room, (int)WiredConditionType.FURNIS_HAVE_AVATARS)
        {
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            foreach (Item item in this.Items)
            {
                foreach (Point coord in item.GetCoords)
                {
                    if (this.RoomInstance.GetGameMap().GetRoomUsers(coord).Count == 0)
                    {
                        return false;
                    }
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
