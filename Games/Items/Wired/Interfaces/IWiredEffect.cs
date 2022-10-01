using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired.Interfaces
{
    public interface IWiredEffect
    {
        void Handle(RoomUser user, Item item);
    }
}
