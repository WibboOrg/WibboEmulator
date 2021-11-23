using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class UserMove : WiredActionBase, IWired, IWiredEffect, IWiredCycleable
    {
        public UserMove(Item item, Room room) : base(item, room, (int)WiredActionType.TOGGLE_FURNI_STATE)
        {
        }

        private void Execute(RoomUser User)
        {
            if (this.Items.Count == 0)
            {
                return;
            }

            Item roomItem = this.Items[0];
            if (roomItem == null)
            {
                return;
            }

            if (roomItem.Coordinate != User.Coordinate)
            {
                User.IsWalking = true;
                User.GoalX = roomItem.GetX;
                User.GoalY = roomItem.GetY;
            }
        }

        public bool OnCycle(RoomUser User, Item Item)
        {
            this.Execute(User);
            return false;
        }

        public void Handle(RoomUser User, Item TriggerItem)
        {
            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, User, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.Execute(User);
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
