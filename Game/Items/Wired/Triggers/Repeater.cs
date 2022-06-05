using Wibbo.Database.Interfaces;
using Wibbo.Game.Rooms;
using Wibbo.Game.Items.Wired.Interfaces;
using System.Data;
using Wibbo.Game.Rooms.Wired;

namespace Wibbo.Game.Items.Wired
{
    public class Repeater : WiredTriggerBase, IWired, IWiredCycleable
    {
        public int DelayCycle { get => (this.IntParams.Count > 0) ? this.IntParams[0] : 0; }

        public Repeater(Item item, Room room) : base(item, room, (int)WiredTriggerType.TRIGGER_PERIODICALLY)
        {
            this.IntParams.Add(0);

            this.RoomInstance.GetWiredHandler().RequestCycle(new WiredCycle(this, null, null));
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, null, null);
            return true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.DelayCycle.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            this.IntParams.Clear();

            if (int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.IntParams.Add(delay);
        }
    }
}
