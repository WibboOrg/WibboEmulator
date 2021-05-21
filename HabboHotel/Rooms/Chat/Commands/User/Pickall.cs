using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Pickall : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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