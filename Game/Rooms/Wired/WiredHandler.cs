using Butterfly.Game.Items;
using Butterfly.Game.Items.Wired;
using Butterfly.Game.Items.Wired.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Butterfly.Game.Rooms.Wired
{
    public class WiredHandler
    {
        private readonly ConcurrentDictionary<Point, List<Item>> _actionStacks;
        private readonly ConcurrentDictionary<Point, List<Item>> _conditionStacks;

        private readonly ConcurrentDictionary<Point, List<RoomUser>> _wiredUsed;

        private readonly List<Point> _specialRandom;
        private readonly Dictionary<Point, int> _specialUnseen;

        private ConcurrentQueue<WiredCycle> _requestingUpdates;

        private readonly Room _room;
        private bool _doCleanup = false;

        public event BotCollisionDelegate TrgBotCollision;
        public event UserAndItemDelegate TrgCollision;
        public event RoomEventDelegate TrgTimer;

        public WiredHandler(Room room)
        {
            this._actionStacks = new ConcurrentDictionary<Point, List<Item>>();
            this._conditionStacks = new ConcurrentDictionary<Point, List<Item>>();
            this._requestingUpdates = new ConcurrentQueue<WiredCycle>();
            this._wiredUsed = new ConcurrentDictionary<Point, List<RoomUser>>();


            this._specialRandom = new List<Point>();
            this._specialUnseen = new Dictionary<Point, int>();

            this._room = room;
        }

        public void AddFurniture(Item item)
        {
            Point itemCoord = item.Coordinate;
            if (WiredUtillity.TypeIsWiredAction(item.GetBaseItem().InteractionType))
            {
                if (this._actionStacks.ContainsKey(itemCoord))
                {
                    this._actionStacks[itemCoord].Add(item);
                }
                else
                {
                    this._actionStacks.TryAdd(itemCoord, new List<Item>() { item });
                }
            }
            else if (WiredUtillity.TypeIsWiredCondition(item.GetBaseItem().InteractionType))
            {
                if (this._conditionStacks.ContainsKey(itemCoord))
                {
                    this._conditionStacks[itemCoord].Add(item);
                }
                else
                {
                    this._conditionStacks.TryAdd(itemCoord, new List<Item>() { item });
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.SPECIALRANDOM)
            {
                if (!this._specialRandom.Contains(itemCoord))
                {
                    this._specialRandom.Add(itemCoord);
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.SPECIALUNSEEN)
            {
                if (!this._specialUnseen.ContainsKey(itemCoord))
                {
                    this._specialUnseen.Add(itemCoord, 0);
                }
            }
        }

        public void RemoveFurniture(Item item)
        {
            Point itemCoord = item.Coordinate;
            if (WiredUtillity.TypeIsWiredAction(item.GetBaseItem().InteractionType))
            {
                Point coordinate = item.Coordinate;
                if (!this._actionStacks.ContainsKey(coordinate))
                {
                    return;
                }
                this._actionStacks[coordinate].Remove(item);
                if (this._actionStacks[coordinate].Count == 0)
                {
                    this._actionStacks.TryRemove(coordinate, out List<Item> NewList);
                }
            }
            else if (WiredUtillity.TypeIsWiredCondition(item.GetBaseItem().InteractionType))
            {
                if (!this._conditionStacks.ContainsKey(itemCoord))
                {
                    return;
                }
                this._conditionStacks[itemCoord].Remove(item);
                if (this._conditionStacks[itemCoord].Count == 0)
                {
                    List<Item> newList = new List<Item>();
                    this._conditionStacks.TryRemove(itemCoord, out newList);
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.SPECIALRANDOM)
            {
                if (this._specialRandom.Contains(itemCoord))
                {
                    this._specialRandom.Remove(itemCoord);
                }
            }
            else if (item.GetBaseItem().InteractionType == InteractionType.SPECIALUNSEEN)
            {
                if (this._specialUnseen.ContainsKey(itemCoord))
                {
                    this._specialUnseen.Remove(itemCoord);
                }
            }
        }

        public void OnCycle()
        {
            if (this._doCleanup)
            {
                this.ClearWired();
            }
            else
            {
                if (this._requestingUpdates.Count > 0)
                {
                    List<WiredCycle> toAdd = new List<WiredCycle>();
                    while (this._requestingUpdates.Count > 0)
                    {
                        if (!this._requestingUpdates.TryDequeue(out WiredCycle handler))
                        {
                            continue;
                        }

                        if (handler.WiredCycleable.Disposed())
                        {
                            continue;
                        }

                        if (handler.OnCycle())
                        {
                            toAdd.Add(handler);
                        }
                    }

                    foreach (WiredCycle cycle in toAdd)
                    {
                        this._requestingUpdates.Enqueue(cycle);
                    }
                }

                this._wiredUsed.Clear();
            }
        }

        private void ClearWired()
        {
            foreach (List<Item> list in this._actionStacks.Values)
            {
                foreach (Item roomItem in list)
                {
                    if (roomItem.WiredHandler != null)
                    {
                        roomItem.WiredHandler.Dispose();
                        roomItem.WiredHandler = null;
                    }
                }
            }

            foreach (List<Item> list in this._conditionStacks.Values)
            {
                foreach (Item roomItem in list)
                {
                    if (roomItem.WiredHandler != null)
                    {
                        roomItem.WiredHandler.Dispose();
                        roomItem.WiredHandler = null;
                    }
                }
            }

            this._conditionStacks.Clear();
            this._actionStacks.Clear();
            this._wiredUsed.Clear();
            this._doCleanup = false;
        }

        public void OnPickall()
        {
            this._doCleanup = true;
        }

        public void ExecutePile(Point coordinate, RoomUser user, Item item)
        {
            if (!this._actionStacks.ContainsKey(coordinate))
            {
                return;
            }

            if (this._wiredUsed.ContainsKey(coordinate))
            {
                if (this._wiredUsed[coordinate].Contains(user))
                {
                    return;
                }
                else
                {
                    this._wiredUsed[coordinate].Add(user);
                }
            }
            else
            {
                this._wiredUsed.TryAdd(coordinate, new List<RoomUser>() { user });
            }

            if (this._conditionStacks.ContainsKey(coordinate))
            {
                List<Item> ConditionStack = this._conditionStacks[coordinate];
                int CycleCountCdt = 0;
                foreach (Item roomItem in ConditionStack.ToArray())
                {
                    CycleCountCdt++;
                    if (CycleCountCdt > 20)
                    {
                        break;
                    }

                    if (roomItem == null || roomItem.WiredHandler == null)
                    {
                        continue;
                    }

                    if (!((IWiredCondition)roomItem.WiredHandler).AllowsExecution(user, item))
                    {
                        return;
                    }
                }
            }

            List<Item> ActionStack = this._actionStacks[coordinate].OrderBy(p => p.Z).ToList();

            if (this._specialRandom.Contains(coordinate))
            {
                int CountAct = ActionStack.Count - 1;

                int RdnWired = ButterflyEnvironment.GetRandomNumber(0, CountAct);
                Item ActRand = ActionStack[RdnWired];
                ((IWiredEffect)ActRand.WiredHandler).Handle(user, item);
            }
            else if (this._specialUnseen.ContainsKey(coordinate))
            {
                int CountAct = ActionStack.Count - 1;

                int NextWired = this._specialUnseen[coordinate];
                if (NextWired > CountAct)
                {
                    NextWired = 0;
                    this._specialUnseen[coordinate] = 0;
                }

                this._specialUnseen[coordinate]++;

                Item ActNext = ActionStack[NextWired];
                if (ActNext != null && ActNext.WiredHandler != null)
                {
                    ((IWiredEffect)ActNext.WiredHandler).Handle(user, item);
                }
            }
            else
            {
                int CycleCount = 0;
                foreach (Item roomItem in ActionStack.ToArray())
                {
                    CycleCount++;

                    if (CycleCount > 20)
                    {
                        break;
                    }

                    if (roomItem != null && roomItem.WiredHandler != null)
                    {
                        ((IWiredEffect)roomItem.WiredHandler).Handle(user, item);
                    }
                }
            }
        }

        public void RequestCycle(WiredCycle handler)
        {
            this._requestingUpdates.Enqueue(handler);
        }

        public Room GetRoom()
        {
            return this._room;
        }

        public void Destroy()
        {
            if (this._actionStacks != null)
            {
                this._actionStacks.Clear();
            }

            if (this._conditionStacks != null)
            {
                this._conditionStacks.Clear();
            }

            if (this._requestingUpdates != null)
            {
                this._requestingUpdates = null;
            }

            this.TrgCollision = null;
            this.TrgBotCollision = null;
            this.TrgTimer = null;
            this._wiredUsed.Clear();
        }

        public void TriggerCollision(RoomUser roomUser, Item Item)
        {
            if (this.TrgCollision != null)
            {
                this.TrgCollision(roomUser, Item);
            }
        }

        public void TriggerBotCollision(RoomUser roomUser, string BotName)
        {
            if (this.TrgBotCollision != null)
            {
                this.TrgBotCollision(roomUser, BotName);
            }
        }

        public void TriggerTimer()
        {
            if (this.TrgTimer != null)
            {
                this.TrgTimer(null, null);
            }
        }
    }
}
