using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Wired.Interfaces
{
    public interface IWiredEffect
    {
        void Handle(RoomUser user, Item item);
    }
}
