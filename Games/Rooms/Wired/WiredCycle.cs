namespace WibboEmulator.Games.Rooms.Wired;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired.Interfaces;

public class WiredCycle
{
    public RoomUser User { get; set; }
    public Item Item { get; set; }
    public IWiredCycleable WiredCycleable { get; set; }
    public int Cycle { get; set; }

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
