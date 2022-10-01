using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Items.Wired.Interfaces
{
    public interface IWiredCondition : IWired
    {
        bool AllowsExecution(RoomUser user, Item item);
    }
}
