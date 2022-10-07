namespace WibboEmulator.Games.Catalog.Utilities;
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
        var pet = new Pet(-1, userId, 0, name, type, race, color, 0, 100, 100, 0, WibboEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 1, -1, false)
        {
            DBState = DatabaseUpdateState.NEEDS_UPDATE
        };

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            pet.PetId = BotPetDao.InsertGetId(dbClient, pet.PetId, pet.Name, pet.Race, pet.Color, pet.OwnerId, pet.Type, pet.CreationStamp);
        }

        return pet;
    }
}
