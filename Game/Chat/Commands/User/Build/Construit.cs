using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Construit : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string Heigth = Params[1];
            if (double.TryParse(Heigth, out double Result))
            {
                if (Result >= 0.01 && Result <= 10)
                {
                    UserRoom.ConstruitEnable = true;
                    UserRoom.ConstruitHeigth = Result;
                }
            }

        }
    }
}
