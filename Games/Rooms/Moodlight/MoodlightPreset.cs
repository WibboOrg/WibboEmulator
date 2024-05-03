namespace WibboEmulator.Games.Rooms.Moodlight;

public class MoodlightPreset(string colorCode, int colorIntensity, bool backgroundOnly)
{
    public string ColorCode { get; set; } = colorCode;
    public int ColorIntensity { get; set; } = colorIntensity;
    public bool BackgroundOnly { get; set; } = backgroundOnly;
}
