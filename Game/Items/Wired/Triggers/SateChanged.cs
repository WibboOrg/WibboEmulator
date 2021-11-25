using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Data;
using Butterfly.Game.Rooms.Wired;

namespace Butterfly.Game.Items.Wired
{
    public class SateChanged : WiredTriggerBase, IWired, IWiredCycleable
    {
        public int DelayCycle { get => this.Delay; }
        private readonly OnItemTrigger delegateFunction;

        public SateChanged(Item item, Room room) : base(item, room, (int)WiredTriggerType.TOGGLE_FURNI)
        {
            this.StuffTypeSelectionEnabled = true;

            this.delegateFunction = new OnItemTrigger(this.Triggered);
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
            {
                this.OnTriggered(user, item);
            }

            return false;
        }

        private void Triggered(object sender, ItemTriggeredArgs e)
        {
            if (this.Delay > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, e.User, e.Item, this.Delay));
            }
            else
            {
                this.OnTriggered(e.User, e.Item);
            }
        }

        private void OnTriggered(RoomUser user, Item item)
        {
            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, item);
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems();

            if (this.Items != null)
            {
                foreach (Item roomItem in this.Items)
                {
                    roomItem.ItemTriggerEventHandler += this.delegateFunction;
                }
            }
        }

        public override void Dispose()
        {
            if (this.Items != null)
            {
                foreach (Item roomItem in this.Items)
                {
                    roomItem.ItemTriggerEventHandler -= this.delegateFunction;
                }
            }

            base.Dispose();
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.ItemInstance.Id, string.Empty, this.DelayCycle.ToString(), false, this.Items);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;

            string triggerItems = row["triggers_item"].ToString();

            if (triggerItems == "")
                return;

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
