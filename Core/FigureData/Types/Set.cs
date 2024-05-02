namespace WibboEmulator.Core.FigureData.Types;

internal sealed class Set
{
    public int Id { get; set; }
    public string Gender { get; set; }
    public int ClubLevel { get; set; }
    public bool Colorable { get; set; }
    public Dictionary<string, Part> Parts { get; set; }

    public Set(int id, string gender, int clubLevel, bool colorable)
    {
        this.Id = id;
        this.Gender = gender;
        this.ClubLevel = clubLevel;
        this.Colorable = colorable;

        this.Parts = [];
    }

}
