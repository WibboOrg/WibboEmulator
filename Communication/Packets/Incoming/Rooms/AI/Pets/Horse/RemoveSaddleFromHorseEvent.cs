using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Catalog.Utilities;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
