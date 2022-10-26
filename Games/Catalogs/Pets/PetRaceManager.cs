namespace WibboEmulator.Games.Catalogs.Pets;

public class PetRaceManager
{
    private readonly List<PetRace> _races = new();

    public static void Init()
    {

    }

    public List<PetRace> GetRacesForRaceId(int raceId) => this._races.Where(race => race.RaceId == raceId).ToList();
}
