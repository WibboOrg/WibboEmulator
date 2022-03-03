using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Pickall : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
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

            Session.GetUser().GetInventoryComponent().AddItemArray(Room.GetRoomItemHandler().RemoveAllFurniture(Session));
            Session.SendPacket(new FurniListUpdateComposer());

        }
    }
}