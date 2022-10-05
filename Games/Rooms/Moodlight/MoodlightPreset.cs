namespace WibboEmulator.Games.Rooms.Moodlight;

public class MoodlightPreset
{
    public string ColorCode { get; set; }
    public int ColorIntensity { get; set; }
    public bool BackgroundOnly { get; set; }

    public MoodlightPreset(string colorCode, int colorIntensity, bool backgroundOnly)
    {
        this.ColorCode = colorCode;
        this.ColorIntensity = colorIntensity;
        this.BackgroundOnly = backgroundOnly;
    }
}
