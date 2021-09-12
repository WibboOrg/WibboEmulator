using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands
{
    public interface IChatCommand
    {
        string PermissionRequired { get; }
        string Parameters { get; }
        string Description { get; }
        //void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params);
        //void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params);
        void Execute(GameClient Session, Room Room, string[] Params);
    }
}
