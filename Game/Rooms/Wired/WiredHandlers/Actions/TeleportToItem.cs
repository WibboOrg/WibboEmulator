using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;
using System.Linq;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class TeleportToItem : WiredActionBase, IWired, IWiredCycleable, IWiredEffect
    {
        public TeleportToItem(Item item, Room room) : base(item, room, (int)WiredActionType.TELEPORT)
        {
        }

        public override bool OnCycle(RoomUser user, Item item)
        {
            if (user == null)
            {
                return false;
            }

            if (this.Items.Count > 1)
            {
                Item roomItem = this.Items[ButterflyEnvironment.GetRandomNumber(0, this.Items.Count - 1)];
                if (roomItem == null)
                {
                    return false;
                }

                if (roomItem.Coordinate != user.Coordinate)
                {
                    this.RoomInstance.GetGameMap().TeleportToItem(user, roomItem);
                }
            }
            else if (this.Items.Count == 1)
            {
                this.RoomInstance.GetGameMap().TeleportToItem(user, Enumerable.First<Item>(this.Items));
            }

            user.ApplyEffect(user.CurrentEffect, true);
            if (user.FreezeEndCounter <= 0)
            {
                user.Freeze = false;
            }

            return false;
        }

        public override void Handle(RoomUser user, Item item)
        {
            if (this.Items.Count == 0 || user == null)
            {
                return;
            }

            user.ApplyEffect(4, true);
            user.Freeze = true;

            base.Handle(user, item);
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.DelayCycle.ToString(), false, this.Items, this.Delay);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;

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
