using MySqlX.XDevAPI.Common;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class ConstruitStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.ConstruitEnable = false;

            Session.SendWhisper("Construit désactivé!");
        }
    }
}
