using Butterfly.Game.Items;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWiredCondition : IWired
    {
        bool AllowsExecution(RoomUser user, Item item);
    }
}
