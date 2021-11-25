using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Wired.Interfaces
{
    public interface IWiredEffect
    {
        void Handle(RoomUser user, Item item);
    }
}
