using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Pickall : IChatCommand
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
            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            if (Room.RoomData.SellPrice > 0)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.pickall", Session.Langue));
                return;
            }

            Session.GetHabbo().GetInventoryComponent().AddItemArray(Room.GetRoomItemHandler().RemoveAllFurniture(Session));
            Session.SendPacket(new FurniListUpdateComposer());

        }
    }
}