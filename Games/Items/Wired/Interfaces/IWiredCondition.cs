using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Items.Wired.Interfaces
{
    public interface IWiredCondition : IWired
    {
        bool AllowsExecution(RoomUser user, Item item);
    }
}
