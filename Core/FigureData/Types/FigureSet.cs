namespace WibboEmulator.Core.FigureData.Types;

internal sealed class FigureSet
{
    public SetType Type { get; private set; }
    public int PalletId { get; private set; }

    public FigureSet(SetType type, int palletId)
    {
        this.Type = type;
        this.PalletId = palletId;

        this.Sets = new Dictionary<int, Set>();
    }

    public Dictionary<int, Set> Sets { get; set; }
}
