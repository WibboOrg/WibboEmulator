namespace WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;

public interface IWiredCondition : IWired
{
    bool AllowsExecution(RoomUser user, Item item);
}
