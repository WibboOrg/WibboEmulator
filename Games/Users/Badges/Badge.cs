namespace WibboEmulator.Games.Users.Badges;

public class Badge(string code, int slot)
{
    public string Code { get; private set; } = code;
    public int Slot { get; set; } = slot;
}
