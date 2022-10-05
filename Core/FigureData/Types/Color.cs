namespace WibboEmulator.Core.FigureData.Types;

public class Color
{
    public int Id { get; private set; }
    public int Index { get; private set; }
    public int ClubLevel { get; private set; }
    public bool Selectable { get; private set; }

    public Color(int id, int index, int clubLevel, bool selectable)
    {
        this.Id = id;
        this.Index = index;
        this.ClubLevel = clubLevel;
        this.Selectable = selectable;
    }
}
