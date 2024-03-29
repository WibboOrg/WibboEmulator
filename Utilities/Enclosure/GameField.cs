namespace WibboEmulator.Utilities.Enclosure;
using System.Drawing;
using WibboEmulator.Utilities.Astar.Algorithm;
using WibboEmulator.Utilities.Enclosure.Algorithm;

public class GameField : IPathNode
{
    private readonly Queue<GametileUpdate> _newEntries = new();
    private byte[,] _currentField;
    private readonly AStarSolver<GameField> _astarSolver;
    private readonly bool _diagonal;
    private GametileUpdate _currentlyChecking;

    public bool this[int y, int x] => y >= 0 && x >= 0 && y <= this._currentField.GetUpperBound(0) && x <= this._currentField.GetUpperBound(1);

    public GameField(byte[,] theArray, bool diagonalAllowed)
    {
        this._currentField = theArray;
        this._diagonal = diagonalAllowed;
        this._astarSolver = new AStarSolver<GameField>(diagonalAllowed, AStarHeuristicType.EXPERIMENTAL_SEARCH, this, theArray.GetUpperBound(1) + 1, theArray.GetUpperBound(0) + 1);
    }

    public void UpdateLocation(int x, int y, byte value) => this._newEntries.Enqueue(new GametileUpdate(x, y, value));

    public List<PointField> DoUpdate()
    {
        var list = new List<PointField>();
        while (this._newEntries.Count > 0)
        {
            this._currentlyChecking = this._newEntries.Dequeue();

            this._currentField[this._currentlyChecking.Y, this._currentlyChecking.X] = this._currentlyChecking.Value;

            var connectedItems = this.GetConnectedItems(this._currentlyChecking);
            if (connectedItems.Count > 1)
            {
                foreach (var nodeList in this.HandleListOfConnectedPoints(connectedItems))
                {
                    if (nodeList.Count >= 4)
                    {
                        var closed = this.FindClosed(nodeList, this._currentlyChecking.Value);
                        if (closed != null)
                        {
                            list.Add(closed);
                        }
                    }
                }
            }
        }
        return list;
    }

    private PointField FindClosed(LinkedList<AStarSolver<GameField>.PathNode> nodeList, byte team)
    {
        var pointField = new PointField(this._currentlyChecking.Value);
        var num1 = int.MaxValue;
        var num2 = int.MinValue;
        var num3 = int.MaxValue;
        var num4 = int.MinValue;
        foreach (var pathNode in nodeList)
        {
            if (pathNode.X < num1)
            {
                num1 = pathNode.X;
            }

            if (pathNode.X > num2)
            {
                num2 = pathNode.X;
            }

            if (pathNode.Y < num3)
            {
                num3 = pathNode.Y;
            }

            if (pathNode.Y > num4)
            {
                num4 = pathNode.Y;
            }
        }
        var x1 = (int)Math.Ceiling((num2 - num1) / 2.0) + num1;
        var y1 = (int)Math.Ceiling((num4 - num3) / 2.0) + num3;
        var list1 = new List<Point>();
        var list2 = new List<Point>
        {
            new(this._currentlyChecking.X, this._currentlyChecking.Y)
        };
        list1.Add(new Point(x1, y1));
        while (list1.Count > 0)
        {
            var p = list1[0];
            var x2 = p.X;
            var y2 = p.Y;
            if (x2 < num1)
            {
                return null;
            }

            if (x2 > num2)
            {
                return null;
            }

            if (y2 < num3)
            {
                return null;
            }

            if (y2 > num4)
            {
                return null;
            }

            Point point;
            if (this[y2 - 1, x2] && this._currentField[y2 - 1, x2] != team)
            {
                point = new Point(x2, y2 - 1);
                if (!list1.Contains(point) && !list2.Contains(point))
                {
                    list1.Add(point);
                }
            }
            if (this[y2 + 1, x2] && this._currentField[y2 + 1, x2] != team)
            {
                point = new Point(x2, y2 + 1);
                if (!list1.Contains(point) && !list2.Contains(point))
                {
                    list1.Add(point);
                }
            }
            if (this[y2, x2 - 1] && this._currentField[y2, x2 - 1] != team)
            {
                point = new Point(x2 - 1, y2);
                if (!list1.Contains(point) && !list2.Contains(point))
                {
                    list1.Add(point);
                }
            }
            if (this[y2, x2 + 1] && this._currentField[y2, x2 + 1] != team)
            {
                point = new Point(x2 + 1, y2);
                if (!list1.Contains(point) && !list2.Contains(point))
                {
                    list1.Add(point);
                }
            }
            if (this.GetValue(p) != team)
            {
                pointField.Add(p);
            }

            list2.Add(p);
            list1.RemoveAt(0);
        }
        return pointField;
    }

    private List<LinkedList<AStarSolver<GameField>.PathNode>> HandleListOfConnectedPoints(List<Point> pointList)
    {
        var list = new List<LinkedList<AStarSolver<GameField>.PathNode>>();
        var num = 0;
        foreach (var inStartNode in pointList)
        {
            ++num;
            if (num == (pointList.Count / 2) + 1)
            {
                return list;
            }

            foreach (var inEndNode in pointList)
            {
                if (!(inStartNode == inEndNode))
                {
                    var linkedList = this._astarSolver.Search(inEndNode, inStartNode);
                    if (linkedList != null)
                    {
                        list.Add(linkedList);
                    }
                }
            }
        }
        return list;
    }

    private List<Point> GetConnectedItems(GametileUpdate update)
    {
        var list = new List<Point>();
        var x = update.X;
        var y = update.Y;
        if (this._diagonal)
        {
            if (this[y - 1, x - 1] && this._currentField[y - 1, x - 1] == update.Value)
            {
                list.Add(new Point(x - 1, y - 1));
            }

            if (this[y - 1, x + 1] && this._currentField[y - 1, x + 1] == update.Value)
            {
                list.Add(new Point(x + 1, y - 1));
            }

            if (this[y + 1, x - 1] && this._currentField[y + 1, x - 1] == update.Value)
            {
                list.Add(new Point(x - 1, y + 1));
            }

            if (this[y + 1, x + 1] && this._currentField[y + 1, x + 1] == update.Value)
            {
                list.Add(new Point(x + 1, y + 1));
            }
        }
        if (this[y - 1, x] && this._currentField[y - 1, x] == update.Value)
        {
            list.Add(new Point(x, y - 1));
        }

        if (this[y + 1, x] && this._currentField[y + 1, x] == update.Value)
        {
            list.Add(new Point(x, y + 1));
        }

        if (this[y, x - 1] && this._currentField[y, x - 1] == update.Value)
        {
            list.Add(new Point(x - 1, y));
        }

        if (this[y, x + 1] && this._currentField[y, x + 1] == update.Value)
        {
            list.Add(new Point(x + 1, y));
        }

        return list;
    }

    public byte GetValue(int x, int y)
    {
        if (this[y, x])
        {
            return this._currentField[y, x];
        }
        else
        {
            return 0;
        }
    }

    public byte GetValue(Point p)
    {
        if (this[p.Y, p.X])
        {
            return this._currentField[p.Y, p.X];
        }
        else
        {
            return 0;
        }
    }

    public bool IsBlocked(int x, int y, bool lastTile)
    {
        if (this._currentlyChecking.X == x && this._currentlyChecking.Y == y)
        {
            return true;
        }
        else
        {
            return this.GetValue(x, y) != this._currentlyChecking.Value;
        }
    }

    public void Destroy() => this._currentField = null;
}
