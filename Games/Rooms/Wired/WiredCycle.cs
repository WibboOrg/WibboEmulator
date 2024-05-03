namespace WibboEmulator.Games.Rooms.Wired;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired.Interfaces;

public class WiredCycle(IWiredCycleable wiredCycleable, RoomUser user, Item item)
{
    public RoomUser User { get; set; } = user;
    public Item Item { get; set; } = item;
    public IWiredCycleable WiredCycleable { get; set; } = wiredCycleable;
    public int Cycle { get; set; } = 0;

    public bool OnCycle()
    {
        this.Cycle++;

        if (this.User != null && this.User.IsDispose)
        {
            this.User = null;
        }

        if (this.WiredCycleable.IsTeleport && this.WiredCycleable.DelayCycle > 1)
        {
            if (this.Cycle == this.WiredCycleable.DelayCycle - 1)
            {
                return this.WiredCycleable.OnCycle(this.User, this.Item);
            }

            else if (this.Cycle == this.WiredCycleable.DelayCycle)
            {
                this.Cycle = 0;

                return this.WiredCycleable.OnCycle(this.User, this.Item);
            }
        }

        if (this.Cycle <= this.WiredCycleable.DelayCycle)
        {
            return true;
        }

        this.Cycle = 0;

        return this.WiredCycleable.OnCycle(this.User, this.Item);
    }
}
