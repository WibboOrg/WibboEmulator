namespace WibboEmulator.Utilities.Astar.Algorithm;
using System.Drawing;

/// <summary>
/// Uses about 50 MB for a 1024x1024 grid.
/// </summary>
public class AStarSolver<TPathNode> where TPathNode : IPathNode
{
    private delegate double CalculateHeuristicDelegate(PathNode inStart, PathNode inEnd);
    private CalculateHeuristicDelegate _calculationMethod;
    private static readonly double SQRT_2 = Math.Sqrt(2);
    private readonly bool _allowDiagonal;
    private PathNode _startNode;
    private PathNode _endNode;
    private bool[,] _closedSet;
    private bool[,] _openSet;
    private PriorityQueueAstar<PathNode, double> _orderedOpenSet;
    private PathNode[,] _searchSpace;
    private int _size;

    public TPathNode SearchSpace { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    /// <summary>
    /// Creates a new AstarSolver
    /// </summary>
    /// <param name="inGrid">The inut grid</param>
    /// <param name="allowDiagonal">Indication if diagonal is allowed</param>
    /// <param name="calculator">The Calculator method</param>
    public AStarSolver(bool allowDiagonal, AStarHeuristicType calculator, TPathNode inGrid, int width, int height)
    {
        this.SetHeuristictype(calculator);
        this._allowDiagonal = allowDiagonal;
        this.PrepareMap(inGrid, width, height);
    }

    private void PrepareMap(TPathNode inGrid, int width, int height)
    {
        this.SearchSpace = inGrid;
        this.Width = width;//inGrid.GetLength(1);
        this.Height = height;//inGrid.GetLength(0);
        this._size = this.Width * this.Height;
        this._searchSpace = new PathNode[this.Height, this.Width];
        this._orderedOpenSet = new PriorityQueueAstar<PathNode, double>(PathNode.Comparer, this.Width + this.Height);

    }

    private void ResetSearchSpace()
    {
        for (var y = 0; y < this.Height; y++)
        {
            for (var x = 0; x < this.Width; x++)
            {
                this._searchSpace[y, x] = new PathNode(x, y, this.SearchSpace);
            }
        }
    }

    /// <summary>
    /// Sets the calculation type
    /// </summary>
    /// <param name="calculator"></param>
    private void SetHeuristictype(AStarHeuristicType calculator)
    {
        switch (calculator)
        {
            case AStarHeuristicType.FAST_SEARCH:
                this._calculationMethod = this.CalculateHeuristicFast;
                break;
            case AStarHeuristicType.BETWEEN:
                this._calculationMethod = this.CalculateHeuristicBetween;
                break;
            case AStarHeuristicType.SHORTEST_PATH:
                this._calculationMethod = this.CalculateHeuristicShortestRoute;
                break;
            case AStarHeuristicType.EXPERIMENTAL_SEARCH:
                this._calculationMethod = this.CalculateHeuristicExperimental;
                break;
        }
    }

    protected virtual double CalculateHeuristicExperimental(PathNode inStart, PathNode inEnd) => this.CalculateHeuristicFast(inStart, inEnd);

    protected virtual double CalculateHeuristicFast(PathNode inStart, PathNode inEnd)
    {

        double dx1 = inStart.X - this._endNode.X;
        double dy1 = inStart.Y - this._endNode.Y;
        var cross = Math.Abs(dx1 - dy1);
        return Math.Ceiling(Math.Abs(inStart.X - inEnd.X) + (double)Math.Abs(inStart.Y - inEnd.Y)) + cross;

    }

    protected virtual double CalculateHeuristicBetween(PathNode inStart, PathNode inEnd)
    {
        double dx1 = inStart.X - this._endNode.X;
        double dy1 = inStart.Y - this._endNode.Y;
        double dx2 = this._startNode.X - this._endNode.X;
        double dy2 = this._startNode.Y - this._endNode.Y;
        var cross = Math.Abs((dx1 * dy2) - (dx2 * dy1));
        return Math.Ceiling(Math.Abs(inStart.X - inEnd.X) + (double)Math.Abs(inStart.Y - inEnd.Y)) + cross;
    }

    protected virtual double CalculateHeuristicShortestRoute(PathNode inStart, PathNode inEnd) => Math.Sqrt(((inStart.X - inEnd.X) * (inStart.X - inEnd.X)) + ((inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y)));

    /// <summary>
    /// Calculates the neighbour distance
    /// </summary>
    /// <param name="inStart">Start node</param>
    /// <param name="inEnd">End node</param>
    /// <returns></returns>
    protected virtual double NeighborDistance(PathNode inStart, PathNode inEnd)
    {
        var diffX = Math.Abs(inStart.X - inEnd.X);
        var diffY = Math.Abs(inStart.Y - inEnd.Y);

        return (diffX + diffY) switch
        {
            1 => 1,
            2 => SQRT_2,
            _ => 0,
        };
    }

    /// <summary>
    /// Returns null, if no path is found. Start- and End-Node are included in returned path. The user context
    /// is passed to IsWalkable().
    /// </summary>
    public LinkedList<PathNode> Search(Point inEndNode, Point inStartNode) //TPathNode inGrid, int width, int height)
    {
        //prepareMap(inGrid, width, height);
        //if (width < inStartNode.X || height < inStartNode.Y)
        //    return null;
        //if (width < inEndNode.X || height < inEndNode.Y)
        //    return null;
        this.ResetSearchSpace();
        this._orderedOpenSet = new PriorityQueueAstar<PathNode, double>(PathNode.Comparer, this.Width + this.Height);

        this._closedSet = new bool[this.Height, this.Width];
        this._openSet = new bool[this.Height, this.Width];

        this._startNode = this._searchSpace[inStartNode.Y, inStartNode.X];
        this._endNode = this._searchSpace[inEndNode.Y, inEndNode.X];



        if (this._startNode == this._endNode)
        {
            return new LinkedList<PathNode>([this._startNode]);
        }

        PathNode[] neighborNodes;
        if (this._allowDiagonal)
        {
            neighborNodes = new PathNode[8];
        }
        else
        {
            neighborNodes = new PathNode[4];
        }

        this._startNode.G = 0;
        this._startNode.Optimal = this._calculationMethod(this._startNode, this._endNode);
        this._startNode.F = this._startNode.Optimal;

        this._orderedOpenSet.Push(this._startNode);
        this._startNode.ExtraWeight = this._size;

        double trailScore;
        bool wasAdded;
        bool scoreResultBetter;
        PathNode y;
        PathNode x;

        while ((x = this._orderedOpenSet.Pop()) != null)
        {
            if (x == this._endNode)
            {
                var result = ReconstructPath(x);

                _ = result.AddLast(this._endNode);

                return result;
            }

            this._closedSet[x.Y, x.X] = true;

            if (this._allowDiagonal)
            {
                this.StoreNeighborNodesDiagonal(x, neighborNodes);
            }
            else
            {
                this.StoreNeighborNodesNoDiagonal(x, neighborNodes);
            }

            for (var i = 0; i < neighborNodes.Length; i++)
            {
                y = neighborNodes[i];

                if (y == null)
                {
                    continue;
                }

                if (y.UserItem.IsBlocked(y.X, y.Y, this._endNode.X == y.X && this._endNode.Y == y.Y))
                {
                    continue;
                }

                if (this._closedSet[y.Y, y.X])
                {
                    continue;
                }

                trailScore = y.G + 1;
                wasAdded = false;

                if (this._openSet[y.Y, y.X] == false)
                {
                    this._openSet[y.Y, y.X] = true;
                    scoreResultBetter = true;
                    wasAdded = true;
                }
                else if (trailScore < y.G)
                {
                    scoreResultBetter = true;
                }
                else
                {
                    scoreResultBetter = false;
                }

                if (scoreResultBetter)
                {
                    y.Parent = x;

                    if (wasAdded)
                    {
                        y.G = trailScore;
                        y.Optimal = this.CalculateHeuristicBetween(y, this._endNode) + (x.ExtraWeight - 10);
                        y.ExtraWeight = x.ExtraWeight - 10;
                        y.F = y.G + y.Optimal;
                        this._orderedOpenSet.Push(y);
                    }

                    else
                    {
                        y.G = trailScore;
                        y.Optimal = this.CalculateHeuristicBetween(y, this._endNode) + (x.ExtraWeight - 10);
                        this._orderedOpenSet.Update(y, y.G + y.Optimal);
                        y.ExtraWeight = x.ExtraWeight - 10;
                    }
                }
            }
        }

        return null;
    }

    private void StoreNeighborNodesDiagonal(PathNode inAround, PathNode[] inNeighbors)
    {
        var x = inAround.X;
        var y = inAround.Y;

        if (x > 0 && y > 0)
        {
            inNeighbors[0] = this._searchSpace[y - 1, x - 1];
        }
        else
        {
            inNeighbors[0] = null;
        }

        if (y > 0)
        {
            inNeighbors[1] = this._searchSpace[y - 1, x];
        }
        else
        {
            inNeighbors[1] = null;
        }

        if (x < this.Width - 1 && y > 0)
        {
            inNeighbors[2] = this._searchSpace[y - 1, x + 1];
        }
        else
        {
            inNeighbors[2] = null;
        }

        if (x > 0)
        {
            inNeighbors[3] = this._searchSpace[y, x - 1];
        }
        else
        {
            inNeighbors[3] = null;
        }

        if (x < this.Width - 1)
        {
            inNeighbors[4] = this._searchSpace[y, x + 1];
        }
        else
        {
            inNeighbors[4] = null;
        }

        if (x > 0 && y < this.Height - 1)
        {
            inNeighbors[5] = this._searchSpace[y + 1, x - 1];

        }
        else
        {
            inNeighbors[5] = null;
        }

        if (y < this.Height - 1)
        {
            inNeighbors[6] = this._searchSpace[y + 1, x];
        }
        else
        {
            inNeighbors[6] = null;
        }

        if (x < this.Width - 1 && y < this.Height - 1)
        {
            inNeighbors[7] = this._searchSpace[y + 1, x + 1];
        }
        else
        {
            inNeighbors[7] = null;
        }
    }
    private void StoreNeighborNodesNoDiagonal(PathNode inAround, PathNode[] inNeighbors)
    {
        var x = inAround.X;
        var y = inAround.Y;

        if (y > 0)
        {
            inNeighbors[0] = this._searchSpace[y - 1, x];
        }
        else
        {
            inNeighbors[0] = null;
        }

        if (x > 0)
        {
            inNeighbors[1] = this._searchSpace[y, x - 1];
        }
        else
        {
            inNeighbors[1] = null;
        }

        if (x < this.Width - 1)
        {
            inNeighbors[2] = this._searchSpace[y, x + 1];
        }
        else
        {
            inNeighbors[2] = null;
        }

        if (y < this.Height - 1)
        {
            inNeighbors[3] = this._searchSpace[y + 1, x];
        }
        else
        {
            inNeighbors[3] = null;
        }
    }

    private static LinkedList<PathNode> ReconstructPath(PathNode current_node)
    {
        var result = new LinkedList<PathNode>();

        ReconstructPathRecursive(current_node, result);

        return result;
    }
    private static void ReconstructPathRecursive(PathNode current_node, LinkedList<PathNode> result)
    {
        var item = current_node;
        _ = result.AddFirst(item);
        while ((item = item.Parent) != null)
        {
            _ = result.AddFirst(item);
        }
    }

    public class PathNode(int inX, int inY, TPathNode inUserContext) : IPathNode, IComparer<PathNode>, IWeightAddable<double>
    {
        public static readonly PathNode Comparer = new(0, 0, default);

        public TPathNode UserItem { get; internal set; } = inUserContext;
        public double G { get; internal set; }
        public double Optimal { get; internal set; }
        public double F { get; internal set; }

        public PathNode Parent { get; set; }

        public bool IsBlocked(int x, int y, bool lastTile) => this.UserItem.IsBlocked(x, y, lastTile);

        public int X { get; internal set; } = inX;
        public int Y { get; internal set; } = inY;
        public int ExtraWeight { get; set; }
        public int Compare(PathNode x, PathNode y)
        {
            if (x.F < y.F)
            {
                return -1;
            }
            else if (x.F > y.F)
            {
                return 1;
            }

            return 0;
        }

        public double WeightChange
        {
            get => this.F;
            set => this.F = value;
        }

        public bool BeenThere { get; set; }
    }
}
