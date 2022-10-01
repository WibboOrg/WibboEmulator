using MySqlX.XDevAPI.Common;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class UseStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().ForceUse = -1;

            Session.SendWhisper("Use d�sactiv�!");
        }
    }
}
