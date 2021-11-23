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

        public bool OnCycle(RoomUser user, Item item)
        {
            this.TeleportUser(user);

            return false;
        }

        private void DoAnimation(RoomUser user)
        {
            user.ApplyEffect(4, true);
            user.Freeze = true;
        }
        private void ResetAnimation(RoomUser user)
        {
            user.ApplyEffect(user.CurrentEffect, true);
            if (user.FreezeEndCounter <= 0)
            {
                user.Freeze = false;
            }
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            if (user == null)
            {
                return;
            }

            this.DoAnimation(user);
            this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, null, this.DelayCycle));
        }

        private void TeleportUser(RoomUser user)
        {
            if (user == null)
            {
                return;
            }

            if (this.Items.Count > 1)
            {
                Item roomItem = this.Items[ButterflyEnvironment.GetRandomNumber(0, this.Items.Count - 1)];
                if (roomItem == null)
                {
                    return;
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

            this.ResetAnimation(user);
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.DelayCycle.ToString(), false, this.Items);
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
