namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal class RemoveSaddleFromHorseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var Room))
        {
            return;
        }

        if (!Room.GetRoomUserManager().TryGetPet(Packet.PopInt(), out var PetUser))
        {
            return;
        }

        if (PetUser.PetData == null || PetUser.PetData.OwnerId != session.GetUser().Id || PetUser.PetData.Type != 13)
        {
            return;
        }

        //Fetch the furniture Id for the pets current saddle.
        var SaddleId = ItemUtility.GetSaddleId(PetUser.PetData.Saddle);

        //Remove the saddle from the pet.
        PetUser.PetData.Saddle = 0;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            BotPetDao.UpdateHaveSaddle(dbClient, PetUser.PetData.PetId, 0);
        }

        //Give the saddle back to the user.
        if (!WibboEnvironment.GetGame().GetItemManager().GetItem(SaddleId, out var ItemData))
        {
            return;
        }

        var Item = ItemFactory.CreateSingleItemNullable(ItemData, session.GetUser(), "");
        if (Item != null)
        {
            session.GetUser().GetInventoryComponent().TryAddItem(Item);
            session.SendPacket(new FurniListNotificationComposer(Item.Id, 1));
            session.SendPacket(new PurchaseOKComposer());
        }

        Room.SendPacket(new UsersComposer(PetUser));
        Room.SendPacket(new PetHorseFigureInformationComposer(PetUser));
    }
}
