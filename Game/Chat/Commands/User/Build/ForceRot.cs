using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class ForceRot : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            int.TryParse(Params[1], out int num);
            if (num <= -1 || num >= 7)
            {
                Session.GetUser().ForceRot = 0;
            }
            else
            {
                Session.GetUser().ForceRot = num;
            }
        }
    }
}
