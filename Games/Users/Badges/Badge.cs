namespace WibboEmulator.Games.Users.Badges;

public class Badge
{
    public string Code { get; private set; }
    public int Slot { get; set; }

    public Badge(string code, int slot)
    {
        this.Code = code;
        this.Slot = slot;
    }
}
