using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Users.Wardrobes
{
    public class WardrobeComponent : IDisposable
    {
        private readonly User _userInstance;
        private readonly List<Wardrobe> _wardrobes;

        public WardrobeComponent(User user)
        {
            this._userInstance = user;
            this._wardrobes = new List<Wardrobe>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            DataTable WardrobeData = UserWardrobeDao.GetAll(dbClient, this._userInstance.Id);

            foreach (DataRow Row in WardrobeData.Rows)
            {
                Wardrobe wardrobe = new Wardrobe(Convert.ToInt32(Row["slot_id"]), Convert.ToString(Row["look"]), Row["gender"].ToString().ToUpper());
                this._wardrobes.Add(wardrobe);
            }
        }

        public List<Wardrobe> GetWardrobes()
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
