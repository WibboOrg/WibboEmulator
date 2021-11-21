using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
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
                Session.GetHabbo().forceRot = 0;
            }
            else
            {
                Session.GetHabbo().forceRot = num;
            }
        }
    }
}
