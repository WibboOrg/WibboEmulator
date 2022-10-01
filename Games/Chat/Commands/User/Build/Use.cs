using MySqlX.XDevAPI.Common;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Use : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string Count = Params[1];
            if (!int.TryParse(Count, out int UseCount))
            {
                return;
            }

            if (UseCount < 0 || UseCount > 100)
            {
                return;
            }

            Session.GetUser().ForceUse = UseCount;

            Session.SendWhisper("Use: " + UseCount);
        }
    }
}
