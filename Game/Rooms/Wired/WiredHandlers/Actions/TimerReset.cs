using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class TimerReset : WiredActionBase, IWiredEffect, IWired, IWiredCycleable
    {
        public TimerReset(Item item, Room room) : base(item, room, (int)WiredActionType.RESET)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.DelayCycle > 0)
            {
                this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, user, TriggerItem, this.DelayCycle));
            }
            else
            {
                this.ResetTimers();
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.ResetTimers();
            return false;
        }

        private void ResetTimers()
        {
            this.RoomInstance.GetWiredHandler().TriggerTimer();
            this.RoomInstance.lastTimerReset = DateTime.Now;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.DelayCycle.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;
        }
    }
}
