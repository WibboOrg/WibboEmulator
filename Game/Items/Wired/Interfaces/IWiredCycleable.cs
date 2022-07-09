using Wibbo.Game.Rooms;

namespace Wibbo.Game.Items.Wired.Interfaces
{
    public interface IWiredCycleable
    {
        bool OnCycle(RoomUser User, Item item);

        bool Disposed();

        int DelayCycle { get; }
    }
}
