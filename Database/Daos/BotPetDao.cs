using Butterfly.Database.Interfaces;
using Butterfly.Game.Pets;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.AI;
using Butterfly.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Database.Daos
{
    class BotPetDao
    {
        internal static void SavePet(IQueryAdapter dbClient, List<RoomUser> petList)
        {
            QueryChunk queryChunk = new QueryChunk();

            foreach (RoomUser petData in petList)
            {
                Pet pet = petData.PetData;
                if (pet.DBState == DatabaseUpdateState.NeedsUpdate)
                {
                    queryChunk.AddParameter(pet.PetId + "name", pet.Name);
                    queryChunk.AddParameter(pet.PetId + "race", pet.Race);
                    queryChunk.AddParameter(pet.PetId + "color", pet.Color);
                    queryChunk.AddQuery("UPDATE `bot_pet` SET room_id = " + pet.RoomId + ", name = @" + pet.PetId + "name, race = @" + pet.PetId + "race, color = @" + pet.PetId + "color, type = " + pet.Type + ", experience = " + pet.Expirience + ", energy = " + pet.Energy + ", nutrition = " + pet.Nutrition + ", respect = " + pet.Respect + ", createstamp = '" + pet.CreationStamp + "', x = " + petData.X + ", Y = " + petData.Y + ", Z = " + petData.Z + " WHERE id = " + pet.PetId);
                }
                else
                {
                    if (petData.BotData.AiType == AIType.RolePlayPet)
                    {
                        continue;
                    }

                    queryChunk.AddQuery("UPDATE `bot_pet` SET x = " + petData.X + ", Y = " + petData.Y + ", Z = " + petData.Z + " WHERE id = " + pet.PetId);
                }

                pet.DBState = DatabaseUpdateState.Updated;
            }
            queryChunk.Execute(dbClient);
            queryChunk.Dispose();
        }

        internal static void UpdateHaveSaddle(IQueryAdapter dbClient, int petId, int statut)
        {
            dbClient.RunQuery("UPDATE `bot_pet` SET have_saddle = '" + statut + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdatePethair(IQueryAdapter dbClient, int petId, int petHair)
        {
            dbClient.RunQuery("UPDATE `bot_pet` SET pethair = '" + petHair + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateHairdye(IQueryAdapter dbClient, int petId, int hairDye)
        {
            dbClient.RunQuery("UPDATE `bot_pet` SET hairdye = '" + hairDye + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateRace(IQueryAdapter dbClient, int petId, string race)
        {
            dbClient.SetQuery("UPDATE `bot_pet` SET race = @race WHERE id = '" + petId + "' LIMIT 1");
            dbClient.AddParameter("race", race);
            dbClient.RunQuery();
        }

        internal static void UpdateAnyoneRide(IQueryAdapter dbClient, int petId, bool anyoneCanRide)
        {
            dbClient.RunQuery("UPDATE `bot_pet` SET anyone_ride = '" + ButterflyEnvironment.BoolToEnum(anyoneCanRide) + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateRoomId(IQueryAdapter dbClient, int petId, int roomId)
        {
            dbClient.RunQuery("UPDATE `bot_pet` SET room_id = '" + roomId + "' WHERE id = '" + petId + "' LIMIT 1");
        }

        internal static void UpdateRoomIdByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.RunQuery("UPDATE `bot_pet` SET room_id = '0' WHERE room_id = '" + roomId + "'");
        }

        internal static int InsertGetId(IQueryAdapter dbClient, int petId, string petName, string petRace, string petColor, int ownerId, int petType, double petCreationStamp)
        {
            dbClient.SetQuery("INSERT INTO `bot_pet` (user_id, name,type, race,color, experience, energy, createstamp) VALUES (" + ownerId + ",@" + petId + "name," + petType + ",@" + petId + "race,@" + petId + "color,0,100,'" + petCreationStamp + "')");
            dbClient.AddParameter(petId + "name", petName);
            dbClient.AddParameter(petId + "race", petRace);
            dbClient.AddParameter(petId + "color", petColor);
            return Convert.ToInt32(dbClient.InsertQuery());
        }

        internal static void InsertDuplicate(IQueryAdapter dbClient, int userId, int roomId, int oldRoomId)
        {
            dbClient.RunQuery("INSERT INTO `bot_pet` (user_id, room_id, name, race, color, type, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride) " +
                "SELECT '" + userId + "', '" + roomId + "', name, race, color, type, experience, energy, nutrition, respect, '" + ButterflyEnvironment.GetUnixTimestamp() + "', x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM `bot_pet` WHERE room_id = '" + oldRoomId + "'");
        }


        internal static void Delete(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("DELETE FROM `bot_pet` WHERE room_id = '0' AND user_id = '" + userId + "'");
        }

        internal static DataTable GetAllByUserId(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM `bot_pet` WHERE user_id = '" + userId + "' AND room_id = 0");
            return dbClient.GetTable();
        }

        internal static DataTable GetAllByRoomId(IQueryAdapter dbClient, int roomId)
        {
            dbClient.SetQuery("SELECT id, user_id, room_id, name, type, race, color, experience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM `bot_pet` WHERE room_id = '" + roomId + "'");
            return dbClient.GetTable();
        }
    }
}