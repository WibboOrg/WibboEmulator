namespace WibboEmulator.Games.LandingView;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

public class LandingViewManager
{
    public List<Promotion> HotelViewPromosIndexers { get; private set; }

    public LandingViewManager() => this.HotelViewPromosIndexers = new List<Promotion>();

    public void Init(IQueryAdapter dbClient)
    {
        this.HotelViewPromosIndexers.Clear();

        var dTable = EmulatorHotelviewPromoDao.GetAll(dbClient);

        foreach (DataRow dRow in dTable.Rows)
        {
            this.HotelViewPromosIndexers.Add(
                new Promotion(Convert.ToInt32(dRow["index"]), (string)dRow["header"], (string)dRow["body"], (string)dRow["button"], Convert.ToInt32(dRow["in_game_promo"]), (string)dRow["special_action"], (string)dRow["image"])
                );
        }
    }

    public int Count() => this.HotelViewPromosIndexers.Count;

}
