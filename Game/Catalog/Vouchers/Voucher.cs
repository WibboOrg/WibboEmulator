using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;

namespace Wibbo.Game.Catalog.Vouchers
{
    public class Voucher
    {
        private string _code;
        private VoucherType _type;
        private int _value;
        private int _currentUses;
        private int _maxUses;

        public Voucher(string Code, string Type, int Value, int CurrentUses, int MaxUses)
        {
            this._code = Code;
            this._type = VoucherUtility.GetType(Type);
            this._value = Value;
            this._currentUses = CurrentUses;
            this._maxUses = MaxUses;
        }

        public void UpdateUses()
        {
            this.CurrentUses += 1;

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            CatalogVoucherDao.Update(dbClient, this._code);
        }

        public string Code
        {
            get => this._code;
            set => this._code = value;
        }

        public VoucherType Type
        {
            get => this._type;
            set => this._type = value;
        }

        public int Value
        {
            get => this._value;
            set => this._value = value;
        }

        public int CurrentUses
        {
            get => this._currentUses;
            set => this._currentUses = value;
        }

        public int MaxUses
        {
            get => this._maxUses;
            set => this._maxUses = value;
        }
    }
}
