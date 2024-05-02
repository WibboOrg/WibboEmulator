namespace WibboEmulator.Games.Catalogs.Utilities;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.Rooms.AI;

public static class PetUtility
{
    public static bool CheckPetName(string petName)
    {
        if (petName.Length is < 1 or > 16)
        {
            return false;
        }

        if (!WibboEnvironment.IsValidAlphaNumeric(petName))
        {
            return false;
        }

        return true;
    }

    public static Pet CreatePet(int userId, string name, int type, string race, string color)
    {
        var pet = new Pet(0, userId, 0, name, type, race, color, 0, 100, 100, 0, WibboEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 1, -1, false);

        using (var dbClient = DatabaseManager.Connection)
        {
            pet.PetId = BotPetDao.InsertGetId(dbClient, pet.Name, pet.Race, pet.Color, pet.OwnerId, pet.Type, pet.CreationStamp);
        }

        return pet;
    }
}
