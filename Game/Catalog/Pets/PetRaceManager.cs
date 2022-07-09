namespace Wibbo.Game.Catalog.Pets
{
    public class PetRaceManager
    {
        private readonly List<PetRace> _races = new List<PetRace>();

        public void Init()
        {

        }

        public List<PetRace> GetRacesForRaceId(int RaceId)
        {
            return this._races.Where(Race => Race.RaceId == RaceId).ToList();
        }
    }
}