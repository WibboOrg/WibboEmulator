namespace WibboEmulator.Games.Banners;

public class Banner(int id, bool haveLayer, bool canTrade)
{
    public int Id { get; private set; } = id;
    public bool HaveLayer { get; private set; } = haveLayer;
    public bool CanTrade { get; private set; } = canTrade;
}
