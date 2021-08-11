using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Pets;
using System;

namespace Butterfly.HabboHotel.Catalog.Utilities
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
            Pet pet = new Pet(404, UserId, 0, Name, Type, Race, Color, 0, 100, 100, 0, ButterflyEnvironment.GetUnixTimestamp(), 0, 0, 0.0, 0, 1, -1, false)
            {
                DBState = DatabaseUpdateState.NeedsUpdate
            };

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO pets (user_id, name,type, race,color, experience, energy, createstamp) VALUES (" + pet.OwnerId + ",@" + pet.PetId + "name," + pet.Type + ",@" + pet.PetId + "race,@" + pet.PetId + "color,0,100,'" + pet.CreationStamp + "')");
                dbClient.AddParameter(pet.PetId + "name", pet.Name);
                dbClient.AddParameter(pet.PetId + "race", pet.Race);
                dbClient.AddParameter(pet.PetId + "color", pet.Color);
                pet.PetId = Convert.ToInt32(dbClient.InsertQuery());
            }

            return pet;
        }
    }
}
