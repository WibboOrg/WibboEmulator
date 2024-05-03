namespace WibboEmulator.Core.FigureData.Types;

internal sealed class Set(int id, string gender, int clubLevel, bool colorable)
{
    public int Id { get; set; } = id;
    public string Gender { get; set; } = gender;
    public int ClubLevel { get; set; } = clubLevel;
    public bool Colorable { get; set; } = colorable;
    public Dictionary<string, Part> Parts { get; set; } = [];
}
