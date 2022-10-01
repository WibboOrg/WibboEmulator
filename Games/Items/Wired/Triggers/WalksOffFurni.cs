using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Game.Items.Wired.Interfaces;
using System.Data;
using WibboEmulator.Game.Rooms.Wired;

namespace WibboEmulator.Game.Items.Wired.Triggers
{
    public class WalksOffFurni : WiredTriggerBase, IWired, IWiredCycleable
    {
        public int DelayCycle { get => this.Delay; }

        private readonly UserAndItemDelegate delegateFunction;

        public WalksOffFurni(Item item, Room room) : base(item, room, (int)WiredTriggerType.AVATAR_WALKS_OFF_FURNI)
        {
            this.delegateFunction = new UserAndItemDelegate(this.OnUserWalksOffFurni);
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            if (user != null)
            {
                this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, item);
            }

            return false;
        }

        private void OnUserWalksOffFurni(RoomUser user, Item item)
        {
            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, item));
            }
            else
            {
                this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, user, item);
            }
        }

        public override void LoadItems(bool inDatabase = false)
        {
            base.LoadItems();

            if (this.Items != null)
            {
                foreach (Item roomItem in this.Items)
                {
                    roomItem.OnUserWalksOffFurni += this.delegateFunction;
                }
            }
        }

        public override void Dispose()
        {
            if (this.Items != null)
            {
                foreach (Item roomItem in this.Items)
                {
                    roomItem.OnUserWalksOffFurni -= this.delegateFunction;
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
