using Wibbo.Communication.Packets.Outgoing.Catalog;
using Wibbo.Communication.Packets.Outgoing.Inventory.Furni;
using Wibbo.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Catalog.Utilities;
using Wibbo.Game.Clients;
using Wibbo.Game.Items;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class RemoveSaddleFromHorseEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.GetRoomUserManager().TryGetPet(Packet.PopInt(), out RoomUser PetUser))
            {
                return;
            }

            if (PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetUser().Id || PetUser.PetData.Type != 13)
            {
                return;
            }

            //Fetch the furniture Id for the pets current saddle.
            int SaddleId = ItemUtility.GetSaddleId(PetUser.PetData.Saddle);

            //Remove the saddle from the pet.
            PetUser.PetData.Saddle = 0;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                BotPetDao.UpdateHaveSaddle(dbClient, PetUser.PetData.PetId, 0);
            }

            //Give the saddle back to the user.
            if (!WibboEnvironment.GetGame().GetItemManager().GetItem(SaddleId, out ItemData ItemData))
            {
                return;
            }

            Item Item = ItemFactory.CreateSingleItemNullable(ItemData, Session.GetUser(), "");
            if (Item != null)
            {
                Session.GetUser().GetInventoryComponent().TryAddItem(Item);
                Session.SendPacket(new FurniListNotificationComposer(Item.Id, 1));
                Session.SendPacket(new PurchaseOKComposer());
            }

            Room.SendPacket(new UsersComposer(PetUser));
            Room.SendPacket(new PetHorseFigureInformationComposer(PetUser));
        }
    }
}
