using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class Coords : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.SendNotification("X: " + UserRoom.X + " - Y: " + UserRoom.Y + " - Z: " + UserRoom.Z + " - Rot: " + UserRoom.RotBody);
        }
    }
}
