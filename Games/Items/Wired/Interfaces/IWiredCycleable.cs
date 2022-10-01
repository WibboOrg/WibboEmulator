using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired.Interfaces
{
    public interface IWiredCycleable
    {
        bool OnCycle(RoomUser User, Item item);

        bool Disposed();

        int DelayCycle { get; }
    }
}
