namespace WibboEmulator.Core.FigureData.Types;

public class Color(int id, int index, int clubLevel, bool selectable)
{
    public int Id { get; private set; } = id;
    public int Index { get; private set; } = index;
    public int ClubLevel { get; private set; } = clubLevel;
    public bool Selectable { get; private set; } = selectable;
}
