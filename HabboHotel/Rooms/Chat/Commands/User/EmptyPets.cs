using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;

using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class EmptyPets : IChatCommand
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
            Session.GetHabbo().GetInventoryComponent().ClearPets();
            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("empty.cleared", Session.Langue));

        }
    }
}
