using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class DeleteStickyNoteEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemDao.Delete(dbClient, roomItem.Id);
            }
        }
    }
}