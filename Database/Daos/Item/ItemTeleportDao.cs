namespace WibboEmulator.Database.Daos.Item;
using System.Data;
using Dapper;

internal sealed class ItemTeleportDao
{
    internal static int GetOne(IDbConnection dbClient, int teleId) => dbClient.QuerySingleOrDefault<int>(
        "SELECT tele_two_id FROM `item_teleport` WHERE tele_one_id = @TeleId",
        new { TeleId = teleId });

    internal static void Insert(IDbConnection dbClient, int newId, int newIdTwo) => dbClient.Execute(
        "INSERT INTO `item_teleport` (tele_one_id, tele_two_id) VALUES ('" + newId + "', '" + newIdTwo + "')");
}