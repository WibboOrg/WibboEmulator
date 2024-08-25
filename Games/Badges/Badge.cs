namespace WibboEmulator.Games.Badges;

public class Badge(string code, bool canTrade, bool canDelete, bool canGive, int amountWinwins)
{
    public string Code { get; private set; } = code;
    public bool CanTrade { get; private set; } = canTrade;
    public bool CanDelete { get; private set; } = canDelete;
    public bool CanGive { get; private set; } = canGive;
    public int AmountWinwins { get; private set; } = amountWinwins;
}
