using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Wired.Interfaces
{
    public interface IWiredCycleable
    {
        bool OnCycle(RoomUser User, Item item);

        bool Disposed();

        int DelayCycle { get; }
    }
}
