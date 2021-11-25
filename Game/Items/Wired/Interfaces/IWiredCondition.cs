using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Wired.Interfaces
{
    public interface IWiredCondition : IWired
    {
        bool AllowsExecution(RoomUser user, Item item);
    }
}
