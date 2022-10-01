using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Construit : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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

            Session.SendWhisper("Construit: " + Result);
        }
    }
}
