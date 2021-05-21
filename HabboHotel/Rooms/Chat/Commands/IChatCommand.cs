using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands
{
    public interface IChatCommand
    {
        void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params);
    }
}
