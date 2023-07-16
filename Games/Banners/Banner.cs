namespace WibboEmulator.Games.Banners;

public class Banner
{
    public int Id { get; private set; }
    public bool HaveLayer { get; private set; }

    public Banner(int id, bool haveLayer)
    {
        this.Id = id;
        this.HaveLayer = haveLayer;
    }
}