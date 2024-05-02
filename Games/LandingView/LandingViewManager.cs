namespace WibboEmulator.Games.LandingView;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class LandingViewManager
{
    public static List<Promotion> HotelViewPromosIndexers { get; private set; } = [];

    public static void Initialize(IDbConnection dbClient)
    {
        HotelViewPromosIndexers.Clear();

        var emulatorLandingViewList = EmulatorHotelviewPromoDao.GetAll(dbClient);

        foreach (var emulatorLandingView in emulatorLandingViewList)
        {
            HotelViewPromosIndexers.Add(
                new Promotion(emulatorLandingView.Index, emulatorLandingView.Header, emulatorLandingView.Body, emulatorLandingView.Button,
                emulatorLandingView.InGamePromo, emulatorLandingView.SpecialAction, emulatorLandingView.Image)
            );
        }
    }

    public static int Count => HotelViewPromosIndexers.Count;

}
