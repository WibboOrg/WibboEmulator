using Wibbo.Game.Rooms;

namespace Wibbo.Game.Items.Wired.Interfaces
{
    public interface IWiredCondition : IWired
    {
        bool AllowsExecution(RoomUser user, Item item);
    }
}
