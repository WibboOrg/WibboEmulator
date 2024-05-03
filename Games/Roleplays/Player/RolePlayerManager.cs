namespace WibboEmulator.Games.Roleplays.Player;
using System.Collections.Concurrent;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;

public class RolePlayerManager(int id, int hopitalId, int prisonId)
{
    private readonly ConcurrentDictionary<int, RolePlayer> _player = new();
    public int PrisonId { get; set; } = prisonId;
    public int HopitalId { get; set; } = hopitalId;

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

        using (var dbClient = DatabaseManager.Connection)
        {
            var rpUser = UserRoleplayDao.GetOne(dbClient, userId, id);
            if (rpUser == null)
            {
                UserRoleplayDao.Insert(dbClient, userId, id);
                player = new RolePlayer(id, userId, 100, 0, 0, 0, 100, 0, 0);
            }
            else
            {
                player = new RolePlayer(id, userId, rpUser.Health, rpUser.Money, rpUser.Munition, rpUser.Exp, rpUser.Energy, rpUser.WeaponFar, rpUser.WeaponCac);
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
