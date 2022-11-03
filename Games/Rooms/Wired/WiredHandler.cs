namespace WibboEmulator.Games.Rooms.Wired;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms.Events;

public class WiredHandler
{
    private readonly ConcurrentDictionary<Point, List<Item>> _actionStacks;
    private readonly ConcurrentDictionary<Point, List<Item>> _conditionStacks;

    private readonly ConcurrentDictionary<Point, List<int>> _wiredUsed;

    private readonly List<Point> _specialRandom;
    private readonly Dictionary<Point, int> _specialUnseen;

    private readonly ConcurrentQueue<WiredCycle> _requestingUpdates;

    private int _tickCounter;
    private bool _doCleanup;

    public event EventHandler<ItemTriggeredEventArgs> TrgBotCollision;
    public event EventHandler<ItemTriggeredEventArgs> TrgCollision;
    public event EventHandler TrgTimer;

    public WiredHandler()
    {
        this._actionStacks = new ConcurrentDictionary<Point, List<Item>>();
        this._conditionStacks = new ConcurrentDictionary<Point, List<Item>>();
        this._requestingUpdates = new ConcurrentQueue<WiredCycle>();
        this._wiredUsed = new ConcurrentDictionary<Point, List<int>>();

        this._specialRandom = new List<Point>();
        this._specialUnseen = new Dictionary<Point, int>();

        this._tickCounter = 0;
    }

    public void AddFurniture(Item item)
    {
        var itemCoord = item.Coordinate;
        if (WiredUtillity.TypeIsWiredAction(item.GetBaseItem().InteractionType))
        {
            if (this._actionStacks.ContainsKey(itemCoord))
            {
                this._actionStacks[itemCoord].Add(item);
            }
            else
            {
                _ = this._actionStacks.TryAdd(itemCoord, new List<Item>() { item });
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
                _ = this._conditionStacks.TryAdd(itemCoord, new List<Item>() { item });
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
        var itemCoord = item.Coordinate;
        if (WiredUtillity.TypeIsWiredAction(item.GetBaseItem().InteractionType))
        {
            var coordinate = item.Coordinate;
            if (!this._actionStacks.ContainsKey(coordinate))
            {
                return;
            }
            _ = this._actionStacks[coordinate].Remove(item);
            if (this._actionStacks[coordinate].Count == 0)
            {
                _ = this._actionStacks.TryRemove(coordinate, out _);
            }
        }
        else if (WiredUtillity.TypeIsWiredCondition(item.GetBaseItem().InteractionType))
        {
            if (!this._conditionStacks.ContainsKey(itemCoord))
            {
                return;
            }
            _ = this._conditionStacks[itemCoord].Remove(item);
            if (this._conditionStacks[itemCoord].Count == 0)
            {
                _ = this._conditionStacks.TryRemove(itemCoord, out _);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIALRANDOM)
        {
            if (this._specialRandom.Contains(itemCoord))
            {
                _ = this._specialRandom.Remove(itemCoord);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIALUNSEEN)
        {
            if (this._specialUnseen.ContainsKey(itemCoord))
            {
                _ = this._specialUnseen.Remove(itemCoord);
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
            if (!this._requestingUpdates.IsEmpty)
            {
                var toAdd = new List<WiredCycle>();
                while (!this._requestingUpdates.IsEmpty)
                {
                    if (!this._requestingUpdates.TryDequeue(out var handler))
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

                foreach (var cycle in toAdd)
                {
                    this._requestingUpdates.Enqueue(cycle);
                }
            }

            this._tickCounter = 0;
            this._wiredUsed.Clear();
        }
    }

    private void ClearWired()
    {
        foreach (var list in this._actionStacks.Values)
        {
            foreach (var roomItem in list)
            {
                if (roomItem.WiredHandler != null)
                {
                    roomItem.WiredHandler.Dispose();
                    roomItem.WiredHandler = null;
                }
            }
        }

        foreach (var list in this._conditionStacks.Values)
        {
            foreach (var roomItem in list)
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

    public void OnPickall() => this._doCleanup = true;

    public void ExecutePile(Point coordinate, RoomUser user, Item item)
    {
        if (this._doCleanup)
        {
            return;
        }

        if (!this._actionStacks.ContainsKey(coordinate))
        {
            return;
        }

        if (user != null && user.IsSpectator)
        {
            return;
        }

        if (this._wiredUsed.ContainsKey(coordinate))
        {
            if (this._wiredUsed[coordinate].Contains(user?.VirtualId ?? 0))
            {
                return;
            }
            else
            {
                this._wiredUsed[coordinate].Add(user?.VirtualId ?? 0);
            }
        }
        else
        {
            _ = this._wiredUsed.TryAdd(coordinate, new() { user?.VirtualId ?? 0 });
        }

        if (this._tickCounter > 1024)
        {
            return;
        }

        this._tickCounter++;

        if (this._conditionStacks.ContainsKey(coordinate))
        {
            var conditionStack = this._conditionStacks[coordinate];
            foreach (var roomItem in conditionStack.Take(20).ToArray())
            {
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

        var actionStack = this._actionStacks[coordinate].OrderBy(p => p.Z).ToList();

        if (this._specialRandom.Contains(coordinate))
        {
            var rdnWired = WibboEnvironment.GetRandomNumber(0, actionStack.Count - 1);
            var actRand = actionStack[rdnWired];
            ((IWiredEffect)actRand.WiredHandler).Handle(user, item);
        }
        else if (this._specialUnseen.ContainsKey(coordinate))
        {
            var countAct = actionStack.Count - 1;

            var nextWired = this._specialUnseen[coordinate];
            if (nextWired > countAct)
            {
                nextWired = 0;
                this._specialUnseen[coordinate] = 0;
            }

            this._specialUnseen[coordinate]++;

            var actNext = actionStack[nextWired];
            if (actNext != null && actNext.WiredHandler != null)
            {
                ((IWiredEffect)actNext.WiredHandler).Handle(user, item);
            }
        }
        else
        {
            foreach (var roomItem in actionStack.Take(20).ToArray())
            {
                if (roomItem != null && roomItem.WiredHandler != null)
                {
                    ((IWiredEffect)roomItem.WiredHandler).Handle(user, item);
                }
            }
        }
    }

    public void RequestCycle(WiredCycle handler) => this._requestingUpdates.Enqueue(handler);

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
            this._requestingUpdates.Clear();
        }

        this.TrgCollision = null;
        this.TrgBotCollision = null;
        this.TrgTimer = null;
        this._wiredUsed.Clear();
    }

    public void TriggerCollision(RoomUser roomUser, Item item) => this.TrgCollision?.Invoke(this, new(roomUser, item));

    public void TriggerBotCollision(RoomUser roomUser, string botName) => this.TrgBotCollision?.Invoke(null, new(roomUser, null, botName));

    public void TriggerTimer() => this.TrgTimer?.Invoke(null, new());
}
