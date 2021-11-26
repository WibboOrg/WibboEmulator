using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
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
                Session.GetHabbo().ForceRot = 0;
            }
            else
            {
                Session.GetHabbo().ForceRot = num;
            }
        }
    }
}
