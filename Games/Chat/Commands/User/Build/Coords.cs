using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Coords : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendNotification("X: " + UserRoom.X + " - Y: " + UserRoom.Y + " - Z: " + UserRoom.Z + " - Rot: " + UserRoom.RotBody);
        }
    }
}
