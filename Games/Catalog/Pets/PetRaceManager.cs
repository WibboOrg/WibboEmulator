namespace WibboEmulator.Games.Catalog.Pets;

public class PetRaceManager
{
    private readonly List<PetRace> _races = new();

    public static void Init()
    {

    }

    public List<PetRace> GetRacesForRaceId(int RaceId) => this._races.Where(Race => Race.RaceId == RaceId).ToList();
}
