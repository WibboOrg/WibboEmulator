namespace WibboEmulator.Database.Daos.Catalog;
using System.Data;
using Dapper;

internal sealed class CatalogPetRaceDao
{
    internal static List<CatalogPetRaceEntity> GetAll(IDbConnection dbClient) => dbClient.Query<CatalogPetRaceEntity>(
        "SELECT `raceid`, `color1`, `color2`, `has1color`, `has2color` FROM `catalog_pet_race`"
    ).ToList();
}

public class CatalogPetRaceEntity
{
    public int Id { get; set; }
    public int RaceId { get; set; }
    public int Color1 { get; set; }
    public int Color2 { get; set; }
    public bool Has1Color { get; set; }
    public bool Has2Color { get; set; }
}
