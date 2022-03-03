using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DeleteStickyNoteEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            int ItemId = Packet.PopInt();
            Item roomItem = room.GetRoomItemHandler().GetItem(ItemId);
            if (roomItem == null || (roomItem.GetBaseItem().InteractionType != InteractionType.POSTIT && roomItem.GetBaseItem().InteractionType != InteractionType.PHOTO))
            {
                return;
            }

            room.GetRoomItemHandler().RemoveFurniture(Session, roomItem.Id);
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.Delete(dbClient, roomItem.Id);
            }
        }
    }
}