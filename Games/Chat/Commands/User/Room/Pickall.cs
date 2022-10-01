using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class Pickall : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Room.RoomData.SellPrice > 0)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.pickall", Session.Langue));
                return;
            }

            Session.GetUser().GetInventoryComponent().AddItemArray(Room.GetRoomItemHandler().RemoveAllFurniture(Session));
            Session.SendPacket(new FurniListUpdateComposer());
        }
    }
}