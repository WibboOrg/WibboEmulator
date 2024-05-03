namespace WibboEmulator.Core.FigureData.Types;

internal sealed class FigureSet(SetType type, int palletId)
{
    public SetType Type { get; private set; } = type;
    public int PalletId { get; private set; } = palletId;

    public Dictionary<int, Set> Sets { get; set; } = [];
}
