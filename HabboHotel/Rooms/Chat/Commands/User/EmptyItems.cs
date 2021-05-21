using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class EmptyItems : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {
            bool EmptyAll = (Params.Length > 1 && Params[1] == "all");

            Session.GetHabbo().GetInventoryComponent().ClearItems(EmptyAll);            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));        }    }}