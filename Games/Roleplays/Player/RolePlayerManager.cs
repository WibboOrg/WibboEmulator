namespace WibboEmulator.Games.Roleplays.Player;
using System.Collections.Concurrent;
using WibboEmulator.Database.Daos.User;

public class RolePlayerManager
{
    private readonly ConcurrentDictionary<int, RolePlayer> _player;
    private readonly int _id;
    public int PrisonId { get; set; }
    public int HopitalId { get; set; }

    public RolePlayerManager(int id, int hopitalId, int prisonId)
    {
        this._id = id;
        this.PrisonId = prisonId;
        this.HopitalId = hopitalId;
        this._player = new ConcurrentDictionary<int, RolePlayer>();
    }

    public void Update(int hopitalId, int prisonId)
    {
        this.PrisonId = prisonId;
        this.HopitalId = hopitalId;
    }

    public void AddPlayer(int userId)
    {
        if (this._player.ContainsKey(userId))
        {
            return;
        }

        RolePlayer player = null;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().Connection())
        {
            var rpUser = UserRoleplayDao.GetOne(dbClient, userId, this._id);
            if (rpUser == null)
            {
                UserRoleplayDao.Insert(dbClient, userId, this._id);
                player = new RolePlayer(this._id, userId, 100, 0, 0, 0, 100, 0, 0);
            }
            else
            {
                player = new RolePlayer(this._id, userId, rpUser.Health, rpUser.Money, rpUser.Munition, rpUser.Exp, rpUser.Energy, rpUser.WeaponFar, rpUser.WeaponCac);
            }
        }

        if (player != null)
        {
            _ = this._player.TryAdd(userId, player);
            player.SendUpdate(true);
            player.LoadInventory();
        }
    }

    public void RemovePlayer(int id)
    {
        var player = this.GetPlayer(id);

        if (player == null)
        {
            return;
        }

        player.Destroy();
        _ = this._player.TryRemove(id, out _);
    }

    public RolePlayer GetPlayer(int id)
    {
        if (!this._player.ContainsKey(id))
        {
            return null;
        }

        _ = this._player.TryGetValue(id, out var player);
        return player;
    }
}
