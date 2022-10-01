using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Wired.Interfaces
{
    public interface IWiredCycleable
    {
        bool OnCycle(RoomUser User, Item item);

        bool Disposed();

        int DelayCycle { get; }
    }
}
