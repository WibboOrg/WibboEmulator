namespace WibboEmulator.Core.FigureData.Types;

public class Palette
{
    public int Id { get; set; }
    public Dictionary<int, Color> Colors { get; set; }

    public Palette(int id)
    {
        this.Id = id;
        this.Colors = new Dictionary<int, Color>();
    }
}
