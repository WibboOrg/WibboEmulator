namespace Wibbo.Game.Navigator
{
    public class TopLevelItem
    {
        public int Id { get; private set; }
        public string SearchCode { get; private set; }
        public string Filter { get; private set; }
        public string Localization { get; private set; }

        public TopLevelItem(int id, string searchCode, string filter, string localization)
        {
            this.Id = id;
            this.SearchCode = searchCode;
            this.Filter = filter;
            this.Localization = localization;
        }
    }
}