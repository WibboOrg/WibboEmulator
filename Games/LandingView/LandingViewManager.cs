using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.LandingView.Promotions;
using System.Data;


namespace WibboEmulator.Games.LandingView
{
    public class LandingViewManager
    {
        public List<Promotion> HotelViewPromosIndexers { get; private set; }

        public LandingViewManager()
        {
            this.HotelViewPromosIndexers = new List<Promotion>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this.HotelViewPromosIndexers.Clear();

            DataTable dTable = EmulatorHotelviewPromoDao.GetAll(dbClient);

            foreach (DataRow dRow in dTable.Rows)
            {
                this.HotelViewPromosIndexers.Add(new Promotion(Convert.ToInt32(dRow[0]), (string)dRow[1], (string)dRow[2], (string)dRow[3], Convert.ToInt32(dRow[4]), (string)dRow[5], (string)dRow[6]));
            }
        }

        public int Count()
        {
            return this.HotelViewPromosIndexers.Count;
        }

    }
}
