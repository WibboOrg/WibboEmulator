using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class ForceRot : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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
