namespace WibboEmulator.Utilities.Astar.Algorithm;

public class PriorityQueueAstar<T, TX>(IComparer<T> comparer, int size) where T : IWeightAddable<TX>
{
    public List<T> InnerList { get; set; } = new List<T>(size);
    protected IComparer<T> Comparer { get; set; } = comparer;

    protected virtual int OnCompare(int i, int j) => this.Comparer.Compare(this.InnerList[i], this.InnerList[j]);

    private int BinarySearch(T value)
    {
        int low = 0, high = this.InnerList.Count - 1;

        while (low <= high)
        {
            var midpoint = (low + high) / 2;

            // check to see if value is equal to item in array
            if (this.Comparer.Compare(value, this.InnerList[midpoint]) == 0)
            {
                return midpoint;
            }
            else if (this.Comparer.Compare(value, this.InnerList[midpoint]) == -1)
            {
                high = midpoint - 1;
            }
            else
            {
                low = midpoint + 1;
            }
        }

        // item was not found
        return low;
    }

    /// <summary>
    /// Push an object onto the PQ
    /// </summary>
    /// <param name="O">The new object</param>
    /// <returns>The index in the list where the object is _now_. This will change when objects are taken from or put onto the PQ.</returns>
    public void Push(T item)
    {
        var location = this.BinarySearch(item);
        this.InnerList.Insert(location, item);
    }

    /// <summary>
    /// Get the smallest object and remove it.
    /// </summary>
    /// <returns>The smallest object</returns>
    public T Pop()
    {
        if (this.InnerList.Count == 0)
        {
            return default;
        }

        var item = this.InnerList[0];
        this.InnerList.RemoveAt(0);
        return item;

    }

    public void Update(T element, TX newValue)
    {
        this.InnerList.RemoveAt(this.BinarySearch(element));
        element.WeightChange = newValue;
        this.Push(element);
    }
}
