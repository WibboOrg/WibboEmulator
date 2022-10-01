using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Coords : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendNotification("X: " + UserRoom.X + " - Y: " + UserRoom.Y + " - Z: " + UserRoom.Z + " - Rot: " + UserRoom.RotBody);
        }
    }
}
