using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class ToggleItemState : WiredActionBase, IWired, IWiredCycleable, IWiredEffect
    {
        public ToggleItemState(Item item, Room room) : base(item, room, (int)WiredActionType.TOGGLE_FURNI_STATE)
        {
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.ToggleItems(user);
            return false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.ToggleItems(user);
            }
        }

        private void ToggleItems(RoomUser user)
        {
            foreach (Item roomItem in this.Items)
            {
                if (roomItem != null)
                {
                    if (user != null && user.GetClient() != null)
                    {
                        roomItem.Interactor.OnTrigger(user.GetClient(), roomItem, 0, true);
                    }
                    else
                    {
                        roomItem.Interactor.OnTrigger(null, roomItem, 0, true);
                    }

                }
            }
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
