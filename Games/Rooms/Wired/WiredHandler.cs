namespace WibboEmulator.Games.Rooms.Wired;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Core.Settings;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.Items.Wired;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms.Events;
using WibboEmulator.Utilities;

public class WiredHandler(Room room)
{
    private const int MAX_TICKS = 1024;

    private readonly ConcurrentDictionary<Point, List<Item>> _actionStacks = new();
    private readonly ConcurrentDictionary<Point, List<Item>> _conditionStacks = new();

    private readonly ConcurrentDictionary<Point, List<int>> _wiredUsed = new();

    private readonly List<Point> _specialRandom = [];
    private readonly List<Point> _specialAnimate = [];
    private readonly List<Point> _specialOrEval = [];
    private readonly Dictionary<Point, int> _specialUnseen = [];

    private readonly ConcurrentQueue<WiredCycle> _requestingUpdates = new();
    private int _tickCounter;
    private bool _doCleanup;
    private DateTime _blockWiredDateTime;
    private bool _blockWired;
    private bool _inRuningWired;

    public bool SecurityEnabled { get; set; } = SettingsManager.GetData<bool>("wired.security.enable");

    public event EventHandler<ItemTriggeredEventArgs> TrgBotCollision;
    public event EventHandler<ItemTriggeredEventArgs> TrgCollision;
    public event EventHandler TrgTimer;

    public void AddFurniture(Item item)
    {
        var itemCoord = item.Coordinate;
        if (WiredUtillity.TypeIsWiredAction(item.ItemData.InteractionType))
        {
            this._actionStacks.AddOrUpdate(itemCoord, _ => [item], (_, existingValue) =>
            {
                existingValue.Add(item);
                return existingValue;
            });
        }
        else if (WiredUtillity.TypeIsWiredCondition(item.ItemData.InteractionType))
        {
            this._conditionStacks.AddOrUpdate(itemCoord, _ => [item], (_, existingValue) =>
            {
                existingValue.Add(item);
                return existingValue;
            });
        }
        else if (item.ItemData.InteractionType == InteractionType.SPECIAL_RANDOM)
        {
            if (!this._specialRandom.Contains(itemCoord))
            {
                this._specialRandom.Add(itemCoord);
            }
        }
        else if (item.ItemData.InteractionType == InteractionType.SPECIAL_UNSEEN)
        {
            this._specialUnseen.TryAdd(itemCoord, 0);
        }
        else if (item.ItemData.InteractionType == InteractionType.SPECIAL_ANIMATE)
        {
            if (!this._specialAnimate.Contains(itemCoord))
            {
                this._specialAnimate.Add(itemCoord);
            }
        }
        else if (item.ItemData.InteractionType == InteractionType.SPECIAL_OR_EVAL)
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

        if (WiredUtillity.TypeIsWiredAction(item.ItemData.InteractionType))
        {
            RemoveFromStack(this._actionStacks, itemCoord, item);
        }
        else if (WiredUtillity.TypeIsWiredCondition(item.ItemData.InteractionType))
        {
            RemoveFromStack(this._conditionStacks, itemCoord, item);
        }
        else
        {
            var interactionType = item.ItemData.InteractionType;

            if (interactionType == InteractionType.SPECIAL_RANDOM && this._specialRandom.Contains(itemCoord))
            {
                _ = this._specialRandom.Remove(itemCoord);
            }
            else if (interactionType == InteractionType.SPECIAL_UNSEEN && this._specialUnseen.ContainsKey(itemCoord))
            {
                _ = this._specialUnseen.Remove(itemCoord);
            }
            else if (interactionType == InteractionType.SPECIAL_ANIMATE && this._specialAnimate.Contains(itemCoord))
            {
                _ = this._specialAnimate.Remove(itemCoord);
            }
            else if (interactionType == InteractionType.SPECIAL_OR_EVAL && this._specialOrEval.Contains(itemCoord))
            {
                _ = this._specialOrEval.Remove(itemCoord);
            }
        }
    }

    private static void RemoveFromStack(ConcurrentDictionary<Point, List<Item>> stack, Point coordinate, Item item)
    {
        if (stack.TryGetValue(coordinate, out var stackItems))
        {
            _ = stack[coordinate].Remove(item);

            if (stackItems.Count == 0)
            {
                _ = stack.TryRemove(coordinate, out _);
            }
        }
    }

    public void OnCycle()
    {
        if (this._inRuningWired)
        {
            return;
        }
        else if (this._doCleanup)
        {
            this.ClearWired();
        }
        else if (this._blockWired)
        {
            var wiredDateTime = DateTime.Now - this._blockWiredDateTime;
            if (wiredDateTime > TimeSpan.FromSeconds(30))
            {
                this._blockWired = false;
            }
        }
        else if (!this._requestingUpdates.IsEmpty)
        {
            var wiredCounter = 0;
            var toAdd = new List<WiredCycle>();
            while (this._requestingUpdates.TryDequeue(out var handler))
            {
                wiredCounter = Interlocked.Increment(ref wiredCounter);

                if (wiredCounter > MAX_TICKS)
                {
                    break;
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
        this._wiredUsed?.Clear();
    }

    private void ClearWired()
    {
        ClearWiredHandlers(this._actionStacks);
        ClearWiredHandlers(this._conditionStacks);

        this._conditionStacks.Clear();
        this._actionStacks.Clear();
        this._wiredUsed.Clear();
        this._doCleanup = false;
    }

    private static void ClearWiredHandlers(ConcurrentDictionary<Point, List<Item>> stacks)
    {
        foreach (var list in stacks.Values)
        {
            var filteredItems = list
                .Where(roomItem => roomItem != null && roomItem.WiredHandler != null)
                .ToList();

            foreach (var roomItem in filteredItems)
            {
                roomItem.WiredHandler?.Dispose();
                roomItem.WiredHandler = null;
            }
        }
    }

    public void OnPickall() => this._doCleanup = true;

    public void ExecutePile(Point coordinate, RoomUser user, Item item, bool ignoreCondition = false)
    {
        if (this._doCleanup || this._blockWired)
        {
            return;
        }

        if (!this._actionStacks.TryGetValue(coordinate, out var actionStacks))
        {
            return;
        }

        if (user?.IsSpectator == true)
        {
            return;
        }

        if (this.SecurityEnabled)
        {
            if (this._wiredUsed.TryGetValue(coordinate, out var usedList))
            {
                if (usedList.Contains(user?.VirtualId ?? 0))
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
                _ = this._wiredUsed.TryAdd(coordinate, [user?.VirtualId ?? 0]);
            }
        }

        this._tickCounter = Interlocked.Increment(ref this._tickCounter);

        if (this._tickCounter > MAX_TICKS)
        {
            this._blockWired = true;
            this._blockWiredDateTime = DateTime.Now;
            room.SendPacket(new BroadcastMessageAlertComposer("Attention la limite d'effets Wired est dépassée, ils sont par conséquent désactivés durant 30 secondes"));
            return;
        }

        if (this._conditionStacks.TryGetValue(coordinate, out var conditionStack) && !ignoreCondition)
        {
            var isOrVal = this._specialOrEval.Contains(coordinate);
            var hasValidCondition = !isOrVal;

            var filteredItems = conditionStack.Take(48)
                .Where(roomItem => roomItem != null && roomItem.WiredHandler != null)
                .ToList();

            foreach (var roomItem in filteredItems)
            {
                var isValidCondition = ((IWiredCondition)roomItem.WiredHandler).AllowsExecution(user, item);

                if (!isValidCondition && !isOrVal)
                {
                    hasValidCondition = false;
                    break;
                }

                if (isValidCondition && isOrVal)
                {
                    hasValidCondition = true;
                    break;
                }
            }

            if (!hasValidCondition)
            {
                return;
            }
        }

        var actionStack = actionStacks.OrderBy(p => p.Z).ToList();

        if (this._specialRandom.Contains(coordinate))
        {
            var actRand = actionStack.GetRandomElement();
            ((IWiredEffect)actRand.WiredHandler).Handle(user, item);
        }
        else if (this._specialUnseen.TryGetValue(coordinate, out var nextWiredIndex))
        {
            var countAct = actionStack.Count - 1;
            var nextWired = nextWiredIndex > countAct ? 0 : nextWiredIndex;
            this._specialUnseen[coordinate] = nextWired + 1;

            var actNext = actionStack[nextWired];
            if (actNext != null && actNext.WiredHandler != null)
            {
                ((IWiredEffect)actNext.WiredHandler).Handle(user, item);
            }
        }
        else
        {
            var filteredItems = actionStack.Take(48)
                .Where(roomItem => roomItem != null && roomItem.WiredHandler != null)
                .ToList();

            this._inRuningWired = true;
            foreach (var roomItem in filteredItems)
            {
                if (this._tickCounter > MAX_TICKS)
                {
                    break;
                }

                if (roomItem.WiredHandler is IWiredEffect wiredHandler)
                {
                    wiredHandler.Handle(user, item);
                }
            }
            this._inRuningWired = false;
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
