namespace WibboEmulator.Games.Banners;

public class Banner
{
    public int Id { get; private set; }
    public bool HaveLayer { get; private set; }
    public bool CanTrade { get; private set; }

    public Banner(int id, bool haveLayer, bool canTrade)
    {
        this.Id = id;
        this.HaveLayer = haveLayer;
        this.CanTrade = canTrade;
    }
}
