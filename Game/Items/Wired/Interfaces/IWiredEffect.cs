using Wibbo.Game.Rooms;

namespace Wibbo.Game.Items.Wired.Interfaces
{
    public interface IWiredEffect
    {
        void Handle(RoomUser user, Item item);
    }
}
