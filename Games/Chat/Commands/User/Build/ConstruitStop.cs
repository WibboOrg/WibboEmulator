using MySqlX.XDevAPI.Common;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ConstruitStop : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.ConstruitEnable = false;

            Session.SendWhisper("Construit d�sactiv�!");
        }
    }
}
