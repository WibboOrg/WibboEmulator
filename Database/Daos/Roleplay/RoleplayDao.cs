namespace WibboEmulator.Database.Daos.Roleplay;

using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class RoleplayDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT owner_id, hopital_id, prison_id FROM `roleplay`");
        return dbClient.GetTable();
    }
}