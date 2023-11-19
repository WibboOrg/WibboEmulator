namespace WibboEmulator.Games.Rooms.Wired;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms.Events;

public class WiredHandler
{
    private readonly Room _roomInstance;

    private readonly ConcurrentDictionary<Point, List<Item>> _actionStacks;
    private readonly ConcurrentDictionary<Point, List<Item>> _conditionStacks;

    private readonly ConcurrentDictionary<Point, List<int>> _wiredUsed;

    private readonly List<Point> _specialRandom;
    private readonly List<Point> _specialAnimate;
    private readonly List<Point> _specialOrEval;
    private readonly Dictionary<Point, int> _specialUnseen;

    private readonly ConcurrentQueue<WiredCycle> _requestingUpdates;
    private int _tickCounter;
    private bool _doCleanup;
    private DateTime _blockWiredDateTime;
    private bool _blockWired;

    public bool SecurityEnabled { get; set; }

    public event EventHandler<ItemTriggeredEventArgs> TrgBotCollision;
    public event EventHandler<ItemTriggeredEventArgs> TrgCollision;
    public event EventHandler TrgTimer;

    public WiredHandler(Room room)
    {
        this._roomInstance = room;
        this._actionStacks = new ConcurrentDictionary<Point, List<Item>>();
        this._conditionStacks = new ConcurrentDictionary<Point, List<Item>>();
        this._requestingUpdates = new ConcurrentQueue<WiredCycle>();
        this._wiredUsed = new ConcurrentDictionary<Point, List<int>>();

        this._specialRandom = new List<Point>();
        this._specialAnimate = new List<Point>();
        this._specialOrEval = new List<Point>();
        this._specialUnseen = new Dictionary<Point, int>();
        this._tickCounter = 0;

        this.SecurityEnabled = WibboEnvironment.GetSettings().GetData<bool>("wired.security.enable");
    }

    public void AddFurniture(Item item)
    {
        var itemCoord = item.Coordinate;
        if (WiredUtillity.TypeIsWiredAction(item.GetBaseItem().InteractionType))
        {
            if (this._actionStacks.TryGetValue(itemCoord, out var value))
            {
                value.Add(item);
            }
            else
            {
                _ = this._actionStacks.TryAdd(itemCoord, new List<Item>() { item });
            }
        }
        else if (WiredUtillity.TypeIsWiredCondition(item.GetBaseItem().InteractionType))
        {
            if (this._conditionStacks.TryGetValue(itemCoord, out var value))
            {
                value.Add(item);
            }
            else
            {
                _ = this._conditionStacks.TryAdd(itemCoord, new List<Item>() { item });
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_RANDOM)
        {
            if (!this._specialRandom.Contains(itemCoord))
            {
                this._specialRandom.Add(itemCoord);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_UNSEEN)
        {
            if (!this._specialUnseen.ContainsKey(itemCoord))
            {
                this._specialUnseen.Add(itemCoord, 0);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_ANIMATE)
        {
            if (!this._specialAnimate.Contains(itemCoord))
            {
                this._specialAnimate.Add(itemCoord);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_OR_EVAL)
        {
            if (!this._specialOrEval.Contains(itemCoord))
            {
                this._specialOrEval.Add(itemCoord);
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
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_RANDOM)
        {
            if (this._specialRandom.Contains(itemCoord))
            {
                _ = this._specialRandom.Remove(itemCoord);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_UNSEEN)
        {
            if (this._specialUnseen.ContainsKey(itemCoord))
            {
                _ = this._specialUnseen.Remove(itemCoord);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_ANIMATE)
        {
            if (this._specialAnimate.Contains(itemCoord))
            {
                _ = this._specialAnimate.Remove(itemCoord);
            }
        }
        else if (item.GetBaseItem().InteractionType == InteractionType.SPECIAL_OR_EVAL)
        {
            if (this._specialOrEval.Contains(itemCoord))
            {
                _ = this._specialOrEval.Remove(itemCoord);
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
            if (this._blockWired)
            {
                var wiredDateTime = DateTime.Now - this._blockWiredDateTime;
                if (wiredDateTime > TimeSpan.FromSeconds(5))
                {
                    this._blockWired = false;
                }
                return;
            }

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

    public void ExecutePile(Point coordinate, RoomUser user, Item item, bool ignoreCondition = false)
    {
        if (this._doCleanup || this._blockWired)
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

        if (this.SecurityEnabled)
        {
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
        }

        if (this._tickCounter > 1024)
        {
            this._blockWired = true;
            this._blockWiredDateTime = DateTime.Now;
            this._roomInstance.SendPacket(new BroadcastMessageAlertComposer("Attention la limite d'effets wired est dépassée, ils sont par conséquent désactivés durant 5 secondes"));
            return;
        }

        this._tickCounter++;

        if (this._conditionStacks.TryGetValue(coordinate, out var conditionStack) && !ignoreCondition)
        {
            var isOrVal = this._specialOrEval.Contains(coordinate);
            var hasValidCondition = false;

            foreach (var roomItem in conditionStack.Take(this.SecurityEnabled ? 20 : 1024).ToArray())
            {
                if (roomItem == null || roomItem.WiredHandler == null)
                {
                    continue;
                }

                var isValidCondition = ((IWiredCondition)roomItem.WiredHandler).AllowsExecution(user, item);

                if (isValidCondition && isOrVal)
                {
                    hasValidCondition = true;
                    break;
                }

                if (!isValidCondition && !isOrVal)
                {
                    hasValidCondition = false;
                    break;
                }
            }

            if (!hasValidCondition)
            {
                return;
            }
        }

        var actionStack = this._actionStacks[coordinate].OrderBy(p => p.Z).ToList();

        if (this._specialRandom.Contains(coordinate))
        {
            var rdnWired = WibboEnvironment.GetRandomNumber(0, actionStack.Count - 1);
            var actRand = actionStack[rdnWired];
            ((IWiredEffect)actRand.WiredHandler).Handle(user, item);
        }
        else if (this._specialUnseen.TryGetValue(coordinate, out var nextWiredIndex))
        {
            var countAct = actionStack.Count - 1;

            var nextWired = nextWiredIndex;
            if (nextWired > countAct)
            {
                nextWired = 0;
            }

            this._specialUnseen[coordinate] = nextWired + 1;

            var actNext = actionStack[nextWired];
            if (actNext != null && actNext.WiredHandler != null)
            {
                ((IWiredEffect)actNext.WiredHandler).Handle(user, item);
            }
        }
        else
        {
            foreach (var roomItem in actionStack.Take(this.SecurityEnabled ? 20 : 1024).ToArray())
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
        this._actionStacks?.Clear();

        this._conditionStacks?.Clear();

        this._requestingUpdates?.Clear();

        this.TrgCollision = null;
        this.TrgBotCollision = null;
        this.TrgTimer = null;
        this._wiredUsed.Clear();
    }

    public bool DisableAnimate(Point coord) => this._specialAnimate.Contains(coord);

    public void TriggerCollision(RoomUser roomUser, Item item) => this.TrgCollision?.Invoke(this, new(roomUser, item));

    public void TriggerBotCollision(RoomUser roomUser, string botName) => this.TrgBotCollision?.Invoke(null, new(roomUser, null, botName));

    public void TriggerTimer() => this.TrgTimer?.Invoke(null, new());
}
