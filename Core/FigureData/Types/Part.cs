namespace WibboEmulator.Core.FigureData.Types;

internal sealed class Part(int id, SetType setType, bool colorable, int index, int colorIndex)
{
    public int Id { get; set; } = id;
    public SetType SetType { get; set; } = setType;
    public bool Colorable { get; set; } = colorable;
    public int Index { get; set; } = index;
    public int ColorIndex { get; set; } = colorIndex;
}
