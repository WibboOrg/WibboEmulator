using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class SetMax : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 1)
                return;

            int.TryParse(Params[1], out int MaxUsers);

            if ((MaxUsers > 75 || MaxUsers <= 0) && !Session.GetUser().HasFuse("fuse_mod"))
            {
                Room.SetMaxUsers(75);
            }
            else
            {
                Room.SetMaxUsers(MaxUsers);
            }
        }
    }
}
