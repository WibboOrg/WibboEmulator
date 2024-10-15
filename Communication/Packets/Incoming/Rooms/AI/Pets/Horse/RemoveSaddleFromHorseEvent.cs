namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.Catalogs.Utilities;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms;

internal sealed class RemoveSaddleFromHorseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        if (!room.RoomUserManager.TryGetPet(packet.PopInt(), out var petUser))
        {
            return;
        }

        if (petUser.PetData == null || petUser.PetData.OwnerId != session.User.Id || petUser.PetData.Type != 13)
        {
            return;
        }

        var saddleId = ItemUtility.GetSaddleId(petUser.PetData.Saddle);

        petUser.PetData.Saddle = 0;

        using var dbClient = DatabaseManager.Connection;

        BotPetDao.UpdateHaveSaddle(dbClient, petUser.PetData.PetId, 0);

        if (!ItemManager.GetItem(saddleId, out var itemData))
        {
            return;
        }

        var item = ItemFactory.CreateSingleItemNullable(dbClient, itemData, session.User, "");
        if (item != null)
        {
            session.User.InventoryComponent.TryAddItem(item);

            session.SendPacket(new PurchaseOKComposer());
        }

        room.SendPacket(new UsersComposer(petUser));
        room.SendPacket(new PetHorseFigureInformationComposer(petUser));
    }
}
