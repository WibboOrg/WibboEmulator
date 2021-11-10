using Butterfly.Game.Items;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWiredEffect
    {
        void Handle(RoomUser user, Item item);
    }
}
