using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Game.Badges
{
    public class BadgeManager
    {
        private List<string> _notAllowed;

        public BadgeManager()
        {
            this._notAllowed = new List<string>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._notAllowed.Clear();

            this._notAllowed.AddRange(new List<string>{ "WBASSO", "ADM", "PRWRD1", "GPHWIB", "wibbo.helpeur", "WIBARC", "CRPOFFI", "ZEERSWS", "PRWRD1", 
                "WBI1", "WBI2", "WBI3", "WBI4", "WBI5", "WBI6", "WBI7", "WBI8", "WBI9", "CASINOB", "WPREMIUM", "VIPFREE", "CCECT01", "CCECT02", "CCECP2022" });
        }

        public bool HaveNotAllowed(string badgeId)
        {
            if (this._notAllowed.Contains(badgeId))
                return true;

            if (badgeId.StartsWith("MRUN"))
                return true;

            return false;
        }

        public List<string> GetNotAllowed()
        {
            return this._notAllowed;
        }
    }
}
