namespace WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public interface IWiredEffect
{
    void Handle(RoomUser user, Item item);
}
