using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Pets;

namespace Butterfly.Game.Catalog.Utilities
{
    public static class PetUtility
    {
        public static bool CheckPetName(string PetName)
        {
            if (PetName.Length < 1 || PetName.Length > 16)
            {
                return false;
            }

            if (!ButterflyEnvironment.IsValidAlphaNumeric(PetName))
            {
                return false;
            }

            return true;
        }

        public static Pet CreatePet(int UserId, string Name, int Type, string Race, string Color)
        {
            Pet pet = new Pet(404, UserId, 0, Name, Type, Race, Color, 0, 100, 100, 0, ButterflyEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 1, -1, false);

            pet.DBState = DatabaseUpdateState.NEEDS_UPDATE;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                pet.PetId = BotPetDao.InsertGetId(dbClient, pet.PetId, pet.Name, pet.Race, pet.Color, pet.OwnerId, pet.Type, pet.CreationStamp);
            }

            return pet;
        }
    }
}
