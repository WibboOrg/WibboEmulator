namespace WibboEmulator.Games.Catalogs.Pets;

public class PetRace(int raceId, int primaryColour, int secondaryColour, bool hasPrimaryColour, bool hasSecondaryColour)
{
    public int RaceId { get; set; } = raceId;
    public int PrimaryColour { get; set; } = primaryColour;
    public int SecondaryColour { get; set; } = secondaryColour;
    public bool HasPrimaryColour { get; set; } = hasPrimaryColour;
    public bool HasSecondaryColour { get; set; } = hasSecondaryColour;
}
