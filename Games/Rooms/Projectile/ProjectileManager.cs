namespace WibboEmulator.Games.Rooms.Projectile;
using System.Collections.Concurrent;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Rooms.Map.Movement;
using WibboEmulator.Utilities;

public class ProjectileManager(Room room)
{
    private readonly object _projectileSync = new();

    private readonly List<ItemTemp> _projectile = [];
    private readonly ConcurrentQueue<ItemTemp> _queueProjectile = new();
    private readonly ServerPacketList _messages = new();

    public void OnCycle()
    {
        if (this._projectile.Count == 0 && this._queueProjectile.IsEmpty)
        {
            return;
        }

        foreach (var item in this._projectile.ToArray())
        {
            if (item == null)
            {
                continue;
            }

            var endProjectile = false;
            var usersTouch = new List<RoomUser>();

            _ = new Point(item.X, item.Y);
            var newX = item.X;
            var newY = item.Y;
            var newZ = item.Z;


            Point newPoint;
            if (item.InteractionType == InteractionTypeTemp.Grenade)
            {
                newPoint = MovementUtility.GetMoveCoord(item.X, item.Y, 1, item.Movement);
                newX = newPoint.X;
                newY = newPoint.Y;

                if (item.Distance > 2)
                {
                    newZ += 1;
                }
                else
                {
                    newZ -= 1;
                }

                if (item.Distance <= 0)
                {
                    usersTouch = room.GameMap.GetNearUsers(new Point(newPoint.X, newPoint.Y), 2);

                    endProjectile = true;
                }

                item.Distance--;
            }
            else
            {
                for (var i = 1; i <= 3; i++)
                {
                    newPoint = MovementUtility.GetMoveCoord(item.X, item.Y, i, item.Movement);

                    usersTouch = room.GameMap.GetRoomUsers(newPoint);

                    foreach (var userTouch in usersTouch)
                    {
                        if (this.CheckUserTouch(userTouch, item))
                        {
                            endProjectile = true;
                        }
                    }

                    if (room.GameMap.CanStackItem(newPoint.X, newPoint.Y, true) && (room.GameMap.SqAbsoluteHeight(newPoint.X, newPoint.Y) <= item.Z + 0.5))
                    {
                        newX = newPoint.X;
                        newY = newPoint.Y;
                    }
                    else
                    {
                        endProjectile = true;
                    }

                    if (endProjectile)
                    {
                        break;
                    }

                    item.Distance--;
                    if (item.Distance <= 0)
                    {
                        endProjectile = true;
                        break;
                    }
                }
            }

            this._messages.Add(new SlideObjectBundleComposer(item.X, item.Y, item.Z, newX, newY, newZ, item.Id));

            item.X = newX;
            item.Y = newY;
            item.Z = newZ;

            if (endProjectile)
            {
                foreach (var userTouch in usersTouch)
                {
                    this.CheckUserHit(userTouch, item);
                }

                this.RemoveProjectile(item);
            }
        }

        var bulletUser = new Dictionary<int, int>();

        if (!this._queueProjectile.IsEmpty)
        {
            var toAdd = new List<ItemTemp>();
            lock (this._projectileSync)
            {
                while (!this._queueProjectile.IsEmpty)
                {
                    if (this._queueProjectile.TryDequeue(out var item))
                    {
                        if (!bulletUser.TryGetValue(item.VirtualUserId, out var value))
                        {
                            bulletUser.Add(item.VirtualUserId, 1);
                            this._projectile.Add(item);
                        }
                        else
                        {
                            bulletUser[item.VirtualUserId] = ++value;

                            toAdd.Add(item);
                        }
                    }
                }
            }

            lock (this._projectileSync)
            {
                foreach (var item in toAdd)
                {
                    this._queueProjectile.Enqueue(item);
                }
            }

            toAdd.Clear();
        }

        bulletUser.Clear();

        room.SendMessage(this._messages);
        this._messages.Clear();
    }

    private void CheckUserHit(RoomUser userTouch, ItemTemp item)
    {
        if (userTouch == null)
        {
            return;
        }

        if (room.IsRoleplay)
        {
            if (userTouch.VirtualId == item.VirtualUserId)
            {
                return;
            }

            if (userTouch.IsBot)
            {
                if (userTouch.BotData.RoleBot == null)
                {
                    return;
                }

                userTouch.BotData.RoleBot.Hit(userTouch, item.Value, room, item.VirtualUserId, item.TeamId);
            }
            else
            {
                var rp = userTouch.Roleplayer;

                if (rp == null)
                {
                    return;
                }

                if (!rp.PvpEnable && rp.AggroTimer == 0)
                {
                    return;
                }

                rp.Hit(userTouch, item.Value, room, true, item.InteractionType == InteractionTypeTemp.ProjectileBot);
            }
        }
        else
        {
            room.WiredHandler.TriggerCollision(userTouch, null);
        }
    }

    private bool CheckUserTouch(RoomUser userTouch, ItemTemp item)
    {
        if (userTouch == null)
        {
            return false;
        }

        if (!room.IsRoleplay)
        {
            return true;
        }

        if (userTouch.VirtualId == item.VirtualUserId)
        {
            return false;
        }

        if (userTouch.IsBot)
        {
            if (userTouch.BotData.RoleBot == null)
            {
                return false;
            }

            if (userTouch.BotData.RoleBot.Dead)
            {
                return false;
            }

            return true;
        }
        else
        {
            var rp = userTouch.Roleplayer;

            if (rp == null)
            {
                return false;
            }

            if ((!rp.PvpEnable && rp.AggroTimer == 0) || rp.Dead || rp.SendPrison)
            {
                return false;
            }

            return true;
        }
    }


    private void RemoveProjectile(ItemTemp item)
    {
        if (!this._projectile.Contains(item))
        {
            return;
        }

        _ = this._projectile.Remove(item);

        room.RoomItemHandling.RemoveTempItem(item.Id);
    }

    public void AddProjectile(int id, int x, int y, double z, MovementDirection movement, int dmg = 0, int distance = 10, int teamId = -1, bool isBot = false)
    {
        var item = room.RoomItemHandling.AddTempItem(id, 77151726, x, y, z, "1", dmg, isBot ? InteractionTypeTemp.ProjectileBot : InteractionTypeTemp.Projectile, movement, distance, teamId);

        lock (this._projectileSync)
        {
            this._queueProjectile.Enqueue(item);
        }
    }
}
