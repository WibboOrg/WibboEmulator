namespace Butterfly.HabboHotel.Navigators
{
    public class TopLevelItem
    {
        private int _id;
        private string _searchCode;
        private string _filter;
        private string _localization;

        public TopLevelItem(int Id, string SearchCode, string Filter, string Localization)
        {
            this._id = Id;
            this._searchCode = SearchCode;
            this._filter = Filter;
            this._localization = Localization;
        }

        public int Id
        {
            get => this._id;
            set => this._id = value;
        }

        public string SearchCode
        {
            get => this._searchCode;
            set => this._searchCode = value;
        }

        public string Filter
        {
            get => this._filter;
            set => this._filter = value;
        }

        public string Localization
        {
            get => this._localization;
            set => this._localization = value;
        }
    }
}