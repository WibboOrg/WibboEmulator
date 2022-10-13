namespace WibboEmulator.Games.Catalog.Pets;

public class PetRace
{
    public PetRace(int raceId, int primaryColour, int secondaryColour, bool hasPrimaryColour, bool hasSecondaryColour)
    {
        this.RaceId = raceId;
        this.PrimaryColour = primaryColour;
        this.SecondaryColour = secondaryColour;
        this.HasPrimaryColour = hasPrimaryColour;
        this.HasSecondaryColour = hasSecondaryColour;
    }

    public int RaceId { get; set; }
    public int PrimaryColour { get; set; }
    public int SecondaryColour { get; set; }
    public bool HasPrimaryColour { get; set; }
    public bool HasSecondaryColour { get; set; }
}
