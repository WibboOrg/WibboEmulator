namespace WibboEmulator.Utilities;

public class SpamDetect(TimeSpan minTimeIncrement, TimeSpan minTimeUnBlock, int maxCountBlock)
{
    private bool _blocked;
    private int _count;
    private DateTime _lastTime;

    public bool CheckIsBlocked()
    {
        var timeSpan = DateTime.Now - this._lastTime;
        if (timeSpan < minTimeIncrement)
        {
            this._count++;
        }

        if (timeSpan > minTimeUnBlock)
        {
            this._count = 0;
            this._blocked = false;
        }
        else if (this._count > maxCountBlock)
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
