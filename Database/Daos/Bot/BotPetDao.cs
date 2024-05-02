namespace WibboEmulator.Database.Daos.Bot;
using System.Data;
using Dapper;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal sealed class BotPetDao
{
    internal static void SavePet(IDbConnection dbClient, List<RoomUser> petList)
    {
        var updatePetList = new List<BotPetEntity>();
        foreach (var petData in petList)
        {
            if (petData.BotData.AiType == BotAIType.RoleplayPet)
            {
                continue;
            }

            var pet = petData.PetData;

            if (pet != null)
            {
                updatePetList.Add(new BotPetEntity
                {
                    Experience = pet.Expirience,
                    Energy = pet.Energy,
                    Nutrition = pet.Nutrition,
                    Respect = pet.Respect,
                    X = pet.X,
                    Y = pet.Y,
                    Z = pet.Z,
                    Id = pet.PetId
                });
            }
        }

        if (updatePetList.Count != 0)
        {
            _ = dbClient.Execute(
                @"UPDATE `bot_pet` 
                SET experience = @Experience, 
                    energy = @Energy,
                    nutrition = @Nutrition,
                    respect = @Respect,
                    x = @X,
                    y = @Y,
                    z = @Z
                WHERE id = @Id",
                updatePetList);
        }
    }

    internal static void UpdateHaveSaddle(IDbConnection dbClient, int petId, int statut) => dbClient.Execute(
        "UPDATE `bot_pet` SET have_saddle = '" + statut + "' WHERE id = '" + petId + "' LIMIT 1");

    internal static void UpdatePethair(IDbConnection dbClient, int petId, int petHair) => dbClient.Execute(
        "UPDATE `bot_pet` SET pethair = '" + petHair + "' WHERE id = '" + petId + "' LIMIT 1");

    internal static void UpdateHairdye(IDbConnection dbClient, int petId, int hairDye) => dbClient.Execute(
        "UPDATE `bot_pet` SET hairdye = '" + hairDye + "' WHERE id = '" + petId + "' LIMIT 1");

    internal static void UpdateRace(IDbConnection dbClient, int petId, string race) => dbClient.Execute(
        "UPDATE `bot_pet` SET race = @race WHERE id = '" + petId + "' LIMIT 1", new { race });

    internal static void UpdateAnyoneRide(IDbConnection dbClient, int petId, bool anyoneCanRide) => dbClient.Execute(
        "UPDATE `bot_pet` SET anyone_ride = '" + (anyoneCanRide ? "1" : "0") + "' WHERE id = '" + petId + "' LIMIT 1");

    internal static void UpdateRoomId(IDbConnection dbClient, int petId, int roomId) => dbClient.Execute(
        "UPDATE `bot_pet` SET room_id = '" + roomId + "' WHERE id = '" + petId + "' LIMIT 1");

    internal static void UpdateRoomIdByRoomId(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "UPDATE `bot_pet` SET room_id = '0' WHERE room_id = '" + roomId + "'");

    internal static int InsertGetId(IDbConnection dbClient, string petName, string petRace, string petColor, int ownerId, int petType, int petCreationStamp) => dbClient.ExecuteScalar<int>(
        @"INSERT INTO bot_pet (user_id, name, type, race, color, experience, energy, createstamp)
        VALUES (@ownerId, @petName, @petType, @petRace, @petColor, 0, 100, @petCreationStamp);
        SELECT LAST_INSERT_ID();",
        new { ownerId, petName, petType, petRace, petColor, petCreationStamp });

    internal static void InsertDuplicate(IDbConnection dbClient, int userId, int roomId, int oldRoomId) => dbClient.Execute(
        "INSERT INTO `bot_pet` (user_id, room_id, name, race, color, type, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride) " +
        "SELECT '" + userId + "', '" + roomId + "', name, race, color, type, experience, energy, nutrition, respect, '" + WibboEnvironment.GetUnixTimestamp() + "', x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM `bot_pet` WHERE room_id = '" + oldRoomId + "'");

    internal static void Delete(IDbConnection dbClient, int userId) => dbClient.Execute(
        "DELETE FROM `bot_pet` WHERE room_id = '0' AND user_id = '" + userId + "'");

    internal static List<BotPetEntity> GetAllByUserId(IDbConnection dbClient, int userId, int limit) => dbClient.Query<BotPetEntity>(
        @"SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride 
        FROM `bot_pet`
        WHERE user_id = @UserId AND room_id = 0 
        LIMIT @Limit;", new { UserId = userId, Limit = limit }
    ).ToList();

    internal static List<BotPetEntity> GetAllByRoomId(IDbConnection dbClient, int roomId) => dbClient.Query<BotPetEntity>(
        @"SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride 
        FROM `bot_pet`
        WHERE room_id = @RoomId;",
        new { RoomId = roomId }
    ).ToList();
}

public class BotPetEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public string Name { get; set; }
    public string Race { get; set; }
    public string Color { get; set; }
    public int Type { get; set; }
    public int Experience { get; set; }
    public int Energy { get; set; }
    public int Nutrition { get; set; }
    public int Respect { get; set; }
    public int CreateStamp { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public int HaveSaddle { get; set; }
    public int HairDye { get; set; }
    public int PetHair { get; set; }
    public bool AnyoneRide { get; set; }
}
