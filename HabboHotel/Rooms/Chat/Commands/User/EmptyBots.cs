using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;

using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class EmptyBots : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            Session.GetHabbo().GetInventoryComponent().ClearBots();
            Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));
        }
    }
}
