namespace Butterfly.HabboHotel.Navigators
{
    public class SearchResultList
    {
        private int _id;
        private string _category;
        private string _categoryName;
        private string _customName;
        private bool _canDoActions;
        private int _colour;
        private int _requiredRank;
        private bool _minimized;
        private NavigatorViewMode _viewMode;
        private NavigatorCategoryType _categoryType;
        private NavigatorSearchAllowance _searchAllowance;
        private int _orderId;

        public SearchResultList(int Id, string Category, string CategoryIdentifier, string PublicName, bool CanDoActions, int Colour, int RequiredRank, bool Minimized, NavigatorViewMode ViewMode, string CategoryType, string SearchAllowance, int OrderId)
        {
            this._id = Id;
            this._category = Category;
            this._categoryName = CategoryIdentifier;
            this._customName = PublicName;
            this._canDoActions = CanDoActions;
            this._colour = Colour;
            this._requiredRank = RequiredRank;
            this._viewMode = ViewMode;
            this._minimized = Minimized;
            this._categoryType = NavigatorCategoryTypeUtility.GetCategoryTypeByString(CategoryType);
            this._searchAllowance = NavigatorSearchAllowanceUtility.GetSearchAllowanceByString(SearchAllowance);
            this._orderId = OrderId;
        }

        public int Id
        {
            get => this._id;
            set => this._id = value;
        }

        public string Category
        {
            get => this._category;
            set => this._category = value;
        }

        public string CategoryIdentifier
        {
            get => this._categoryName;
            set => this._categoryName = value;
        }

        public string PublicName
        {
            get => this._customName;
            set => this._customName = value;
        }

        public bool Minimized
        {
            get => this._minimized;
            set => this._minimized = value;
        }

        public bool CanDoActions
        {
            get => this._canDoActions;
            set => this._canDoActions = value;
        }

        public int Colour
        {
            get => this._colour;
            set => this._colour = value;
        }

        public int RequiredRank
        {
            get => this._requiredRank;
            set => this._requiredRank = value;
        }

        public NavigatorViewMode ViewMode
        {
            get => this._viewMode;
            set => this._viewMode = value;
        }

        public NavigatorCategoryType CategoryType
        {
            get => this._categoryType;
            set => this._categoryType = value;
        }

        public NavigatorSearchAllowance SearchAllowance
        {
            get => this._searchAllowance;
            set => this._searchAllowance = value;
        }

        public int OrderId
        {
            get => this._orderId;
            set => this._orderId = value;
        }
    }
}
