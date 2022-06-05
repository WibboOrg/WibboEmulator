using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using System.Data;

namespace Wibbo.Game.Users.Wardrobes
{
    public class WardrobeComponent : IDisposable
    {
        private static readonly int MAX_SLOT = 24;
        private readonly User _userInstance;
        private readonly Dictionary<int, Wardrobe> _wardrobes;

        public WardrobeComponent(User user)
        {
            this._userInstance = user;
            this._wardrobes = new Dictionary<int, Wardrobe>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            DataTable WardrobeData = UserWardrobeDao.GetAll(dbClient, this._userInstance.Id);

            foreach (DataRow Row in WardrobeData.Rows)
            {
                int slotId = Convert.ToInt32(Row["slot_id"]);

                if (this._wardrobes.ContainsKey(slotId))
                    continue;

                if (slotId < 1 || slotId > MAX_SLOT)
                    continue;

                Wardrobe wardrobe = new Wardrobe(slotId, Row["look"].ToString(), Row["gender"].ToString().ToUpper());
                this._wardrobes.Add(slotId, wardrobe);
            }
        }

        internal void AddWardobe(string look, string gender, int slotId)
        {
            if (slotId < 1 || slotId > MAX_SLOT)
                return;

            gender = gender.ToUpper();

            if (gender != "M" && gender != "F")
                return;

            if (this._wardrobes.ContainsKey(slotId))
                this._wardrobes.Remove(slotId);

            look = WibboEnvironment.GetFigureManager().ProcessFigure(look, gender, true);

            Wardrobe wardrobe = new Wardrobe(slotId, look, gender);
            this._wardrobes.Add(slotId, wardrobe);

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                UserWardrobeDao.Insert(dbClient, this._userInstance.Id, slotId, look, gender.ToUpper());
        }

        public Dictionary<int, Wardrobe> GetWardrobes()
        {
            return this._wardrobes;
        }

        public void Dispose()
        {
            this._wardrobes.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
