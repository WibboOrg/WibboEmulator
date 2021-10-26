using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class PetDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET have_saddle = '1' WHERE id = '" + PetUser.PetData.PetId + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET have_saddle = '2' WHERE id = '" + PetUser.PetData.PetId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET pethair = '" + PetUser.PetData.PetHair + "' WHERE id = '" + PetUser.PetData.PetId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET hairdye = '" + PetUser.PetData.HairDye + "' WHERE id = '" + PetUser.PetData.PetId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET race = '" + PetUser.PetData.Race + "' WHERE id = '" + PetUser.PetData.PetId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET anyone_ride = '" + ButterflyEnvironment.BoolToEnum(Pet.PetData.AnyoneCanRide) + "' WHERE id = '" + PetId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET have_saddle = '0' WHERE id = '" + PetUser.PetData.PetId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET room_id = '0' WHERE id = '" + pet.PetId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET room_id = '" + Pet.RoomId + "' WHERE id = '" + Pet.PetId + "' LIMIT 1");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE pets SET room_id = '0' WHERE room_id = '" + RoomId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO pets (user_id, name,type, race,color, experience, energy, createstamp) VALUES (" + pet.OwnerId + ",@" + pet.PetId + "name," + pet.Type + ",@" + pet.PetId + "race,@" + pet.PetId + "color,0,100,'" + pet.CreationStamp + "')");
            dbClient.AddParameter(pet.PetId + "name", pet.Name);
            dbClient.AddParameter(pet.PetId + "race", pet.Race);
            dbClient.AddParameter(pet.PetId + "color", pet.Color);
            pet.PetId = Convert.ToInt32(dbClient.InsertQuery());
        }


        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO pets (user_id, room_id, name, race, color, type, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride) " +
                "SELECT '" + Session.GetHabbo().Id + "', '" + RoomId + "', name, race, color, type, experience, energy, nutrition, respect, '" + ButterflyEnvironment.GetUnixTimestamp() + "', x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM pets WHERE room_id = '" + OldRoomId + "'");
        }


        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM pets WHERE room_id = '0' AND user_id = '" + this.UserId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM pets WHERE user_id = '" + this.UserId + "' AND room_id = 0");
            table2 = dbClient.GetTable();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM pets WHERE room_id = '" + this.Id + "'");
            DataTable table = dbClient.GetTable();
        }
    }
}