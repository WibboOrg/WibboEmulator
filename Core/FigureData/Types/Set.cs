namespace WibboEmulator.Core.FigureData.Types;

internal class Set
{
    public int Id;
    public string Gender;
    public int ClubLevel;
    public bool Colorable;

    public Set(int id, string gender, int clubLevel, bool colorable)
    {
        this.Id = id;
        this.Gender = gender;
        this.ClubLevel = clubLevel;
        this.Colorable = colorable;

        this.Parts = new Dictionary<string, Part>();
    }

    public Dictionary<string, Part> Parts { get; set; }
}
