using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class EmptyItems : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            bool EmptyAll = (Params.Length > 1 && Params[1] == "all");

            Session.GetHabbo().GetInventoryComponent().ClearItems(EmptyAll);
            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));
        }
    }
}
