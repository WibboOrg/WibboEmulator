using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class EmptyItems : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            bool EmptyAll = (Params.Length > 1 && Params[1] == "all");

            Session.GetUser().GetInventoryComponent().ClearItems(EmptyAll);
            Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));
        }
    }
}
