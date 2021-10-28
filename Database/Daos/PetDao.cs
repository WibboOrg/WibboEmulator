using System;
using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class PetDao
    {
        internal static void UpdateHaveSaddleOne(IQueryAdapter dbClient, int petId)
        {
            dbClient.RunQuery("UPDATE pets SET have_saddle = '1' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateHaveSaddleOff(IQueryAdapter dbClient, int petId)
        {
            dbClient.RunQuery("UPDATE pets SET have_saddle = '2' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdatePethair(IQueryAdapter dbClient, int petId, int petHair)
        {
            dbClient.RunQuery("UPDATE pets SET pethair = '" + petHair + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateHairdye(IQueryAdapter dbClient, int petId, int hairDye)
        {
            dbClient.RunQuery("UPDATE pets SET hairdye = '" + hairDye + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateRace(IQueryAdapter dbClient, int petId, int race)
        {
            dbClient.RunQuery("UPDATE pets SET race = '" + race + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateAnyoneRide(IQueryAdapter dbClient, int petId, bool anyoneCanRide)
        {
            dbClient.RunQuery("UPDATE pets SET anyone_ride = '" + ButterflyEnvironment.BoolToEnum(anyoneCanRide) + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateRoomId(IQueryAdapter dbClient, int petId)
        {
            dbClient.RunQuery("UPDATE pets SET room_id = '0' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void Update(IQueryAdapter dbClient, int petId, int roomId)
        {
            dbClient.RunQuery("UPDATE pets SET room_id = '" + roomId + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateRoomIdByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("UPDATE pets SET room_id = '0' WHERE room_id = '" + roomId + "'");
        }

        internal static int InsertGetId(IQueryAdapter dbClient, int petId, string petName, string petRace, string petColor, int ownerId, int petType, int petCreationStamp)
        {
            dbClient.SetQuery("INSERT INTO pets (user_id, name,type, race,color, experience, energy, createstamp) VALUES (" + ownerId + ",@" + petId + "name," + petType + ",@" + petId + "race,@" + petId + "color,0,100,'" + petCreationStamp + "')");
            dbClient.AddParameter(petId + "name", petName);
            dbClient.AddParameter(petId + "race", petRace);
            dbClient.AddParameter(petId + "color", petColor);
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void InsertDuplicate(IQueryAdapter dbClient, int userId, int roomId, int oldRoomId)
        {
            dbClient.RunQuery("INSERT INTO pets (user_id, room_id, name, race, color, type, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride) " +
                "SELECT '" + userId + "', '" + roomId + "', name, race, color, type, experience, energy, nutrition, respect, '" + ButterflyEnvironment.GetUnixTimestamp() + "', x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM pets WHERE room_id = '" + oldRoomId + "'");
        }


        internal static void Delete(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE FROM pets WHERE room_id = '0' AND user_id = '" + userId + "'");
        }

        internal static DataTable GetAllByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM pets WHERE user_id = '" + userId + "' AND room_id = 0");
            return dbClient.GetTable();
        }

        internal static DataTable GetAllByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM pets WHERE room_id = '" + roomId + "'");
            return dbClient.GetTable();
        }
    }
}