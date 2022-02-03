using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Items;
using Butterfly.Game.Roleplay.Player;
using Butterfly.Game.Rooms.Map.Movement;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Butterfly.Game.Rooms.Projectile
{
    public class ProjectileManager
    {
        private readonly List<ItemTemp> _projectile;
        private readonly ConcurrentQueue<ItemTemp> _queueProjectile;
        private readonly Room _room;

        private readonly List<ServerPacket> _messages;

        public ProjectileManager(Room room)
        {
            this._projectile = new List<ItemTemp>();
            this._queueProjectile = new ConcurrentQueue<ItemTemp>();
            this._room = room;
            this._messages = new List<ServerPacket>();
        }

        public void OnCycle()
        {
            if (this._projectile.Count == 0 && this._queueProjectile.Count == 0)
            {
                return;
            }

            foreach (ItemTemp Item in this._projectile.ToArray())
            {
                if (Item == null)
                {
                    continue;
                }

                bool EndProjectile = false;
                List<RoomUser> UsersTouch = new List<RoomUser>();
                Point newPoint = new Point(Item.X, Item.Y);
                int newX = Item.X;
                int newY = Item.Y;
                double newZ = Item.Z;

                if (Item.InteractionType == InteractionTypeTemp.GRENADE)
                {
                    newPoint = MovementUtility.GetMoveCoord(Item.X, Item.Y, 1, Item.Movement);
                    newX = newPoint.X;
                    newY = newPoint.Y;

                    if (Item.Distance > 2)
                    {
                        newZ += 1;
                    }
                    else
                    {
                        newZ -= 1;
                    }

                    if (Item.Distance <= 0)
                    {
                        UsersTouch = this._room.GetGameMap().GetNearUsers(new Point(newPoint.X, newPoint.Y), 2);

                        EndProjectile = true;
                    }

                    Item.Distance--;
                }
                else
                {
                    for (int i = 1; i <= 3; i++)
                    {
                        newPoint = MovementUtility.GetMoveCoord(Item.X, Item.Y, i, Item.Movement);

                        UsersTouch = this._room.GetGameMap().GetRoomUsers(newPoint);

                        foreach (RoomUser UserTouch in UsersTouch)
                        {
                            if (this.CheckUserTouch(UserTouch, Item))
                            {
                                EndProjectile = true;
                            }
                        }

                        if ((this._room.GetGameMap().CanStackItem(newPoint.X, newPoint.Y, true) && (this._room.GetGameMap().SqAbsoluteHeight(newPoint.X, newPoint.Y) <= Item.Z + 0.5)))
                        {
                            newX = newPoint.X;
                            newY = newPoint.Y;
                        }
                        else
                        {
                            EndProjectile = true;
                        }

                        if (EndProjectile)
                        {
                            break;
                        }

                        Item.Distance--;
                        if (Item.Distance <= 0)
                        {
                            EndProjectile = true;
                            break;
                        }
                    }
                }

                this._messages.Add(new SlideObjectBundleComposer(Item.X, Item.Y, Item.Z, newX, newY, newZ, Item.Id));

                Item.X = newX;
                Item.Y = newY;
                Item.Z = newZ;

                if (EndProjectile)
                {
                    foreach (RoomUser UserTouch in UsersTouch)
                    {
                        this.CheckUserHit(UserTouch, Item);
                    }

                    this.RemoveProjectile(Item);
                }
            }

            Dictionary<int, int> BulletUser = new Dictionary<int, int>();

            if (this._queueProjectile.Count > 0)
            {
                List<ItemTemp> toAdd = new List<ItemTemp>();
                while (this._queueProjectile.Count > 0)
                {
                    if (this._queueProjectile.TryDequeue(out ItemTemp Item))
                    {
                        if (!BulletUser.ContainsKey(Item.VirtualUserId))
                        {
                            BulletUser.Add(Item.VirtualUserId, 1);
                            this._projectile.Add(Item);
                        }
                        else
                        {
                            BulletUser[Item.VirtualUserId]++;

                            toAdd.Add(Item);
                        }
                    }

                }
                foreach (ItemTemp Item in toAdd)
                {
                    this._queueProjectile.Enqueue(Item);
                }

                toAdd.Clear();
            }

            BulletUser.Clear();

            this._room.SendMessage(this._messages);
            this._messages.Clear();
        }

        private void CheckUserHit(RoomUser UserTouch, ItemTemp Item)
        {
            if (UserTouch == null)
            {
                return;
            }

            if (this._room.IsRoleplay)
            {
                if (UserTouch.VirtualId == Item.VirtualUserId)
                {
                    return;
                }

                if (UserTouch.IsBot)
                {
                    if (UserTouch.BotData.RoleBot == null)
                    {
                        return;
                    }

                    UserTouch.BotData.RoleBot.Hit(UserTouch, Item.Value, this._room, Item.VirtualUserId, Item.TeamId);
                }
                else
                {
                    RolePlayer Rp = UserTouch.Roleplayer;

                    if (Rp == null)
                    {
                        return;
                    }

                    if (!Rp.PvpEnable && Rp.AggroTimer == 0)
                    {
                        return;
                    }

                    Rp.Hit(UserTouch, Item.Value, this._room, true, (Item.InteractionType == InteractionTypeTemp.PROJECTILE_BOT));
                }
            }
            else
            {
                this._room.GetWiredHandler().TriggerCollision(UserTouch, null);
            }
        }

        private bool CheckUserTouch(RoomUser UserTouch, ItemTemp Item)
        {
            if (UserTouch == null)
            {
                return false;
            }

            if (!this._room.IsRoleplay)
            {
                return true;
            }

            if (UserTouch.VirtualId == Item.VirtualUserId)
            {
                return false;
            }

            if (UserTouch.IsBot)
            {
                if (UserTouch.BotData.RoleBot == null)
                {
                    return false;
                }

                if (UserTouch.BotData.RoleBot.Dead)
                {
                    return false;
                }

                return true;
            }
            else
            {
                RolePlayer Rp = UserTouch.Roleplayer;

                if (Rp == null)
                {
                    return false;
                }

                if ((!Rp.PvpEnable && Rp.AggroTimer == 0) || Rp.Dead || Rp.SendPrison)
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

            this._projectile.Remove(item);

            this._room.GetRoomItemHandler().RemoveTempItem(item.Id);
        }

        public void AddProjectile(int id, int x, int y, double z, MovementDirection movement, int dmg = 0, int distance = 10, int teamId = -1, bool isBot = false)
        {
            ItemTemp Item = this._room.GetRoomItemHandler().AddTempItem(id, 77151726, x, y, z, "1", dmg, (isBot) ? InteractionTypeTemp.PROJECTILE_BOT : InteractionTypeTemp.PROJECTILE, movement, distance, teamId);
            this._queueProjectile.Enqueue(Item);
        }

        public void AddGrenade(int id, int x, int y, double z, MovementDirection movement, int dmg = 0, int distance = 10, int teamId = -1)
        {
            ItemTemp Item = this._room.GetRoomItemHandler().AddTempItem(id, 48741061, x, y, z, "1", dmg, InteractionTypeTemp.GRENADE, movement, 4, teamId);
            this._queueProjectile.Enqueue(Item);
        }
    }
}
