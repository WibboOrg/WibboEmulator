using MySqlX.XDevAPI.Common;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class UseStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetUser().ForceUse = -1;

            Session.SendWhisper("Use désactiver!");
        }
    }
}
