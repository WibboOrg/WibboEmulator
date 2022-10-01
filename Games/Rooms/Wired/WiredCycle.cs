using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired.Interfaces;

namespace WibboEmulator.Games.Rooms.Wired
{
    public class WiredCycle
    {
        public RoomUser User;
        public Item Item;
        public IWiredCycleable WiredCycleable;
        public int Cycle;

        public WiredCycle(IWiredCycleable wiredCycleable, RoomUser user, Item item)
        {
            this.WiredCycleable = wiredCycleable;
            this.User = user;
            this.Item = item;
            this.Cycle = 0;
        }

        public bool OnCycle()
        {
            this.Cycle++;

            if (this.Cycle <= this.WiredCycleable.DelayCycle)
            {
                return true;
            }

            this.Cycle = 0;

            if (this.User == null || (this.User != null && this.User.IsDispose))
            {
                this.User = null;
            }

            return this.WiredCycleable.OnCycle(this.User, this.Item);
        }
    }
}