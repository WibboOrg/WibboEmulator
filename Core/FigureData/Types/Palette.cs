namespace WibboEmulator.Core.FigureData.Types;

public class Palette(int id)
{
    public int Id { get; set; } = id;
    public Dictionary<int, Color> Colors { get; set; } = [];
}
