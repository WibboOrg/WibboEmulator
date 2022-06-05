namespace Butterfly.Core.FigureData.Types
{
    internal class FigureSet
    {
        public SetType Type;
        public int PalletId;

        private Dictionary<int, Set> _sets;

        public FigureSet(SetType type, int palletId)
        {
            this.Type = type;
            this.PalletId = palletId;

            this._sets = new Dictionary<int, Set>();
        }

        public Dictionary<int, Set> Sets
        {
            get => this._sets;
            set => this._sets = value;
        }
    }
}