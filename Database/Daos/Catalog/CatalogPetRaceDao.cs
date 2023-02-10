namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using WibboEmulator.Database.Interfaces;

internal sealed class CatalogPetRaceDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `raceid`, `color1`, `color2`, `has1color`, `has2color` FROM `catalog_pet_race`");
        return dbClient.GetTable();
    }
}
