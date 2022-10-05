namespace WibboEmulator.Games.Catalog.Utilities;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.Pets;

public static class PetUtility
{
    public static bool CheckPetName(string PetName)
    {
        if (PetName.Length is < 1 or > 16)
        {
            return false;
        }

        if (!WibboEnvironment.IsValidAlphaNumeric(PetName))
        {
            return false;
        }

        return true;
    }

    public static Pet CreatePet(int UserId, string Name, int Type, string Race, string Color)
    {
        var pet = new Pet(404, UserId, 0, Name, Type, Race, Color, 0, 100, 100, 0, WibboEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 1, -1, false);

        pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            pet.PetId = BotPetDao.InsertGetId(dbClient, pet.PetId, pet.Name, pet.Race, pet.Color, pet.OwnerId, pet.Type, pet.CreationStamp);
        }

        return pet;
    }
}
