namespace WibboEmulator.Utilities.Enclosure;
using System.Drawing;

public class PointField
{
    private static readonly Point BadPoint = new(-1, -1);
    private Point _mostLeft = BadPoint;
    private Point _mostTop = BadPoint;
    private Point _mostRight = BadPoint;
    private Point _mostDown = BadPoint;
    private readonly List<Point> _pointList;

    public byte ForValue { get; private set; }

    static PointField()
    {
    }

    public PointField(byte forValue)
    {
        this._pointList = [];
        this.ForValue = forValue;
    }

    public List<Point> Points => this._pointList;

    public void Add(Point p)
    {
        if (this._mostLeft == BadPoint)
        {
            this._mostLeft = p;
        }

        if (this._mostRight == BadPoint)
        {
            this._mostRight = p;
        }

        if (this._mostTop == BadPoint)
        {
            this._mostTop = p;
        }

        if (this._mostDown == BadPoint)
        {
            this._mostDown = p;
        }

        if (p.X < this._mostLeft.X)
        {
            this._mostLeft = p;
        }

        if (p.X > this._mostRight.X)
        {
            this._mostRight = p;
        }

        if (p.Y > this._mostTop.Y)
        {
            this._mostTop = p;
        }

        if (p.Y < this._mostDown.Y)
        {
            this._mostDown = p;
        }

        this._pointList.Add(p);
    }
}
