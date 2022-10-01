using MySqlX.XDevAPI.Common;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class ForceRot : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            int.TryParse(Params[1], out int result);
            if (result <= -1 || result >= 7)
            {
                Session.GetUser().ForceRot = 0;
            }
            else
            {
                Session.GetUser().ForceRot = result;
            }

            Session.SendWhisper("Rot: " + result);
        }
    }
}
