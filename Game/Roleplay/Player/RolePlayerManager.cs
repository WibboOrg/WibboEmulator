using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Roleplay.Player;
using System.Collections.Concurrent;
using System.Data;

namespace Butterfly.Game.Roleplay
{
    public class RolePlayerManager
    {
        private readonly ConcurrentDictionary<int, RolePlayer> _player;
        private readonly int _id;
        public int PrisonId;
        public int HopitalId;

        public RolePlayerManager(int Id, int HopitalId, int PrisonId)
        {
            this._id = Id;
            this.PrisonId = PrisonId;
            this.HopitalId = HopitalId;
            this._player = new ConcurrentDictionary<int, RolePlayer>();
        }

        public void Update(int HopitalId, int PrisonId)
        {
            this.PrisonId = PrisonId;
            this.HopitalId = HopitalId;
        }

        public void AddPlayer(int UserId)
        {
            if (this._player.ContainsKey(UserId))
            {
                return;
            }

            RolePlayer player = null;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataRow dRow = UserRoleplayDao.GetOne(dbClient, UserId, this._id);
                if (dRow == null)
                {
                    UserRoleplayDao.Insert(dbClient, UserId, this._id);
                    player = new RolePlayer(this._id, UserId, 100, 0, 0, 0, 100, 0, 0);
                }
                else
                {
                    player = new RolePlayer(this._id, UserId, Convert.ToInt32(dRow["health"]), Convert.ToInt32(dRow["money"]), Convert.ToInt32(dRow["munition"]), Convert.ToInt32(dRow["exp"]), Convert.ToInt32(dRow["energy"]), Convert.ToInt32(dRow["weapon_far"]), Convert.ToInt32(dRow["weapon_cac"]));
                }
            }

            if (player != null)
            {
                this._player.TryAdd(UserId, player);
                player.SendUpdate(true);
                player.LoadInventory();
            }
        }

        public void RemovePlayer(int Id)
        {
            RolePlayer player = this.GetPlayer(Id);

            if (player == null)
            {
                return;
            }

            player.Destroy();
            this._player.TryRemove(Id, out player);
        }

        public RolePlayer GetPlayer(int Id)
        {
            if (!this._player.ContainsKey(Id))
            {
                return null;
            }

            this._player.TryGetValue(Id, out RolePlayer player);
            return player;
        }
    }
}
