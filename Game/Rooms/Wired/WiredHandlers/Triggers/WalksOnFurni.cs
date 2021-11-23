using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class WalksOnFurni : WiredTriggerBase, IWired, IWiredCycleable
    {
        private readonly UserAndItemDelegate delegateFunction;
        public int DelayCycle { get => this.Delay; }

        public WalksOnFurni(Item item, Room room) : base (item, room, (int)WiredTriggerType.AVATAR_WALKS_ON_FURNI)
        {
            this.StuffTypeSelectionEnabled = true;

            this.delegateFunction = new UserAndItemDelegate(this.OnUserWalksOnFurni);
        }

        public bool OnCycle(RoomUser User, Item Item)
        {
            if (User != null)
            {
                this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, User, Item);
            }

            return false;
        }

        private void OnUserWalksOnFurni(RoomUser user, Item item)
        {
            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, item, this.DelayCycle));
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
                    roomItem.OnUserWalksOnFurni += this.delegateFunction;
                }
            }
        }

        public override void Dispose()
        {
            if (this.Items != null)
            {
                foreach (Item roomItem in this.Items)
                {
                    roomItem.OnUserWalksOnFurni -= this.delegateFunction;
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
