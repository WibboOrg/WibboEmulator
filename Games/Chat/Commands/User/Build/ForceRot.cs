using MySqlX.XDevAPI.Common;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
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
