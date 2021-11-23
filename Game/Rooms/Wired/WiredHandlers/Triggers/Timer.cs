using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Triggers
{
    public class Timer : WiredTriggerBase, IWired, IWiredCycleable
    {
        public int DelayCycle { get => (this.IntParams.Count > 0) ? this.IntParams[0] : 0; }
        private readonly RoomEventDelegate delegateFunction;

        public Timer(Item item, Room room) : base(item, room, (int)WiredTriggerType.TRIGGER_ONCE)
        {
            this.delegateFunction = new RoomEventDelegate(this.ResetTimer);
            this.RoomInstance.GetWiredHandler().TrgTimer += this.delegateFunction;
        }

        public void ResetTimer(object sender, EventArgs e)
        {
            this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, null, null, this.DelayCycle));
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, null, null);
            return false;
        }

        public override void Dispose()
        {
            this.RoomInstance.GetWiredHandler().TrgTimer -= this.delegateFunction;
            
            base.Dispose();
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.DelayCycle.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.IntParams.Add(delay);
        }
    }
}
