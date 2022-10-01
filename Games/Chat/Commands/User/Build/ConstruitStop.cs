using MySqlX.XDevAPI.Common;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ConstruitStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.ConstruitEnable = false;

            Session.SendWhisper("Construit d�sactiv�!");
        }
    }
}
