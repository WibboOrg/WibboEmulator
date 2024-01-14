namespace WibboEmulator.Games.LandingView;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public class LandingViewManager
{
    public List<Promotion> HotelViewPromosIndexers { get; private set; }

    public LandingViewManager() => this.HotelViewPromosIndexers = new List<Promotion>();

    public void Init(IDbConnection dbClient)
    {
        this.HotelViewPromosIndexers.Clear();

        var emulatorLandingViewList = EmulatorHotelviewPromoDao.GetAll(dbClient);

        if (emulatorLandingViewList.Count == 0)
        {
            return;
        }

        foreach (var emulatorLandingView in emulatorLandingViewList)
        {
            this.HotelViewPromosIndexers.Add(
                new Promotion(emulatorLandingView.Index, emulatorLandingView.Header, emulatorLandingView.Body, emulatorLandingView.Button,
                emulatorLandingView.InGamePromo, emulatorLandingView.SpecialAction, emulatorLandingView.Image)
            );
        }
    }

    public int Count() => this.HotelViewPromosIndexers.Count;

}
