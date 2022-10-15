namespace WibboEmulator.Utilities;

public class SpamDetect
{
    private bool _blocked;
    private int _count;
    private DateTime _lastTime;

    private readonly TimeSpan _minTimeIncrement;
    private readonly TimeSpan _minTimeUnBlock;
    private readonly int _maxCountBlock;

    public SpamDetect(TimeSpan minTimeIncrement, TimeSpan minTimeUnBlock, int maxCountBlock)
    {
        this._minTimeIncrement = minTimeIncrement;
        this._minTimeUnBlock = minTimeUnBlock;
        this._maxCountBlock = maxCountBlock;
    }

    public bool CheckIsBlocked()
    {
        var timeSpan = DateTime.Now - this._lastTime;
        if (timeSpan < this._minTimeIncrement)
        {
            this._count++;
        }

        if (timeSpan > this._minTimeUnBlock)
        {
            this._count = 0;
            this._blocked = false;
        }
        else if (this._count > this._maxCountBlock)
        {
            if (!this._blocked)
            {
                this._blocked = true;
            }

            return true;
        }

        this._lastTime = DateTime.Now;

        return false;
    }
}
