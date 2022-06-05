using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Pets;

namespace Wibbo.Game.Catalog.Utilities
{
    public static class PetUtility
    {
        public static bool CheckPetName(string PetName)
        {
            if (PetName.Length < 1 || PetName.Length > 16)
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
            Pet pet = new Pet(404, UserId, 0, Name, Type, Race, Color, 0, 100, 100, 0, WibboEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 1, -1, false);

            pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                pet.PetId = BotPetDao.InsertGetId(dbClient, pet.PetId, pet.Name, pet.Race, pet.Color, pet.OwnerId, pet.Type, pet.CreationStamp);
            }

            return pet;
        }
    }
}
