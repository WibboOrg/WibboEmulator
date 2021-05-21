using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWiredCondition : IWired
    {
        bool AllowsExecution(RoomUser user, Item item);
    }
}
