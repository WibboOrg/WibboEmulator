using Butterfly.HabboHotel.Items;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWiredEffect
    {
        void Handle(RoomUser user, Item item);
    }
}
