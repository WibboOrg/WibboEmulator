using AStar.Algorithm;
using System.Drawing;

namespace Astar.Algorithm
{
    /// <summary>
    /// Uses about 50 MB for a 1024x1024 grid.
    /// </summary>
    public class AStarSolver<TPathNode> where TPathNode : IPathNode
    {
        private delegate double CalculateHeuristicDelegate(PathNode inStart, PathNode inEnd);
        private CalculateHeuristicDelegate CalculationMethod;
        private static readonly double SQRT_2 = Math.Sqrt(2);
        public double tieBreaker;
        private readonly bool AllowDiagonal;
        private PathNode startNode;
        private PathNode endNode;
        private bool[,] mClosedSet;
        private bool[,] mOpenSet;
        private PriorityQueue<PathNode, double> mOrderedOpenSet;
        private PathNode[,] mSearchSpace;
        private int nodes;
        private int Size;

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
            this.AllowDiagonal = allowDiagonal;
            this.PrepareMap(inGrid, width, height);
        }

        private void PrepareMap(TPathNode inGrid, int width, int height)
        {
            this.SearchSpace = inGrid;
            this.Width = width;//inGrid.GetLength(1);
            this.Height = height;//inGrid.GetLength(0);
            this.Size = this.Width * this.Height;
            this.mSearchSpace = new PathNode[this.Height, this.Width];
            this.mOrderedOpenSet = new PriorityQueue<PathNode, double>(PathNode.Comparer, this.Width + this.Height);

        }

        private void ResetSearchSpace()
        {
            for (int y = 0; y < this.Height; y++)
            {
                for (int x = 0; x < this.Width; x++)
                {
                    this.mSearchSpace[y, x] = new PathNode(x, y, this.SearchSpace);
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
                case AStarHeuristicType.FAST_SEARCH: this.CalculationMethod = this.CalculateHeuristicFast; break;
                case AStarHeuristicType.BETWEEN: this.CalculationMethod = this.CalculateHeuristicBetween; break;
                case AStarHeuristicType.SHORTEST_PATH: this.CalculationMethod = this.CalculateHeuristicShortestRoute; break;
                case AStarHeuristicType.EXPERIMENTAL_SEARCH: this.CalculationMethod = this.CalculateHeuristicExperimental; break;
            }
        }

        protected virtual double CalculateHeuristicExperimental(PathNode inStart, PathNode inEnd) => this.CalculateHeuristicFast(inStart, inEnd);

        protected virtual double CalculateHeuristicFast(PathNode inStart, PathNode inEnd)
        {

            double dx1 = inStart.X - this.endNode.X;
            double dy1 = inStart.Y - this.endNode.Y;
            double cross = Math.Abs(dx1 - dy1);
            return Math.Ceiling(Math.Abs(inStart.X - inEnd.X) + (double)Math.Abs(inStart.Y - inEnd.Y)) + cross;

        }

        protected virtual double CalculateHeuristicBetween(PathNode inStart, PathNode inEnd)
        {
            double dx1 = inStart.X - this.endNode.X;
            double dy1 = inStart.Y - this.endNode.Y;
            double dx2 = this.startNode.X - this.endNode.X;
            double dy2 = this.startNode.Y - this.endNode.Y;
            double cross = Math.Abs(dx1 * dy2 - dx2 * dy1);
            return Math.Ceiling(Math.Abs(inStart.X - inEnd.X) + (double)Math.Abs(inStart.Y - inEnd.Y)) + cross;
        }

        protected virtual double CalculateHeuristicShortestRoute(PathNode inStart, PathNode inEnd) => Math.Sqrt((inStart.X - inEnd.X) * (inStart.X - inEnd.X) + (inStart.Y - inEnd.Y) * (inStart.Y - inEnd.Y));

        /// <summary>
        /// Calculates the neighbour distance
        /// </summary>
        /// <param name="inStart">Start node</param>
        /// <param name="inEnd">End node</param>
        /// <returns></returns>
        protected virtual double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            int diffX = Math.Abs(inStart.X - inEnd.X);
            int diffY = Math.Abs(inStart.Y - inEnd.Y);

            switch (diffX + diffY)
            {
                case 1: return 1;
                case 2: return SQRT_2;
                default:
                    throw new ApplicationException();
            }
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
            this.mOrderedOpenSet = new PriorityQueue<PathNode, double>(PathNode.Comparer, this.Width + this.Height);

            this.mClosedSet = new bool[this.Height, this.Width];
            this.mOpenSet = new bool[this.Height, this.Width];

            this.startNode = this.mSearchSpace[inStartNode.Y, inStartNode.X];
            this.endNode = this.mSearchSpace[inEndNode.Y, inEndNode.X];



            if (this.startNode == this.endNode)
            {
                return new LinkedList<PathNode>(new PathNode[] { this.startNode });
            }

            PathNode[] neighborNodes;
            if (this.AllowDiagonal)
            {
                neighborNodes = new PathNode[8];
            }
            else
            {
                neighborNodes = new PathNode[4];
            }

            this.tieBreaker = 0;

            this.startNode.G = 0;
            this.startNode.Optimal = this.CalculationMethod(this.startNode, this.endNode);
            this.tieBreaker = 1d / this.startNode.Optimal;
            this.startNode.F = this.startNode.Optimal;

            this.mOrderedOpenSet.Push(this.startNode);
            this.startNode.extraWeight = this.Size;
            this.nodes = 0;

            double trailScore;
            bool wasAdded;
            bool scoreResultBetter;
            PathNode y;
            PathNode x;

            while ((x = this.mOrderedOpenSet.Pop()) != null)
            {
                if (x == this.endNode)
                {
                    LinkedList<PathNode> result = this.ReconstructPath(x);

                    result.AddLast(this.endNode);

                    return result;
                }

                this.mClosedSet[x.Y, x.X] = true;

                if (this.AllowDiagonal)
                {
                    this.StoreNeighborNodesDiagonal(x, neighborNodes);
                }
                else
                {
                    this.StoreNeighborNodesNoDiagonal(x, neighborNodes);
                }

                for (int i = 0; i < neighborNodes.Length; i++)
                {
                    y = neighborNodes[i];

                    if (y == null)
                    {
                        continue;
                    }

                    if (y.UserItem.IsBlocked(y.X, y.Y, (this.endNode.X == y.X && this.endNode.Y == y.Y)))
                    {
                        continue;
                    }

                    if (this.mClosedSet[y.Y, y.X])
                    {
                        continue;
                    }

                    this.nodes++;

                    trailScore = y.G + 1;
                    wasAdded = false;

                    if (this.mOpenSet[y.Y, y.X] == false)
                    {
                        this.mOpenSet[y.Y, y.X] = true;
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
                        y.parent = x;

                        if (wasAdded)
                        {
                            y.G = trailScore;
                            y.Optimal = this.CalculateHeuristicBetween(y, this.endNode) + (x.extraWeight - 10);
                            y.extraWeight = x.extraWeight - 10;
                            y.F = y.G + y.Optimal;
                            this.mOrderedOpenSet.Push(y);
                        }

                        else
                        {
                            y.G = trailScore;
                            y.Optimal = this.CalculateHeuristicBetween(y, this.endNode) + (x.extraWeight - 10);
                            this.mOrderedOpenSet.Update(y, y.G + y.Optimal);
                            y.extraWeight = x.extraWeight - 10;
                        }
                    }
                }
            }

            return null;
        }

        private void StoreNeighborNodesDiagonal(PathNode inAround, PathNode[] inNeighbors)
        {
            int x = inAround.X;
            int y = inAround.Y;

            if ((x > 0) && (y > 0))
            {
                inNeighbors[0] = this.mSearchSpace[y - 1, x - 1];
            }
            else
            {
                inNeighbors[0] = null;
            }

            if (y > 0)
            {
                inNeighbors[1] = this.mSearchSpace[y - 1, x];
            }
            else
            {
                inNeighbors[1] = null;
            }

            if ((x < this.Width - 1) && (y > 0))
            {
                inNeighbors[2] = this.mSearchSpace[y - 1, x + 1];
            }
            else
            {
                inNeighbors[2] = null;
            }

            if (x > 0)
            {
                inNeighbors[3] = this.mSearchSpace[y, x - 1];
            }
            else
            {
                inNeighbors[3] = null;
            }

            if (x < this.Width - 1)
            {
                inNeighbors[4] = this.mSearchSpace[y, x + 1];
            }
            else
            {
                inNeighbors[4] = null;
            }

            if ((x > 0) && (y < this.Height - 1))
            {
                inNeighbors[5] = this.mSearchSpace[y + 1, x - 1];

            }
            else
            {
                inNeighbors[5] = null;
            }

            if (y < this.Height - 1)
            {
                inNeighbors[6] = this.mSearchSpace[y + 1, x];
            }
            else
            {
                inNeighbors[6] = null;
            }

            if ((x < this.Width - 1) && (y < this.Height - 1))
            {
                inNeighbors[7] = this.mSearchSpace[y + 1, x + 1];
            }
            else
            {
                inNeighbors[7] = null;
            }
        }
        private void StoreNeighborNodesNoDiagonal(PathNode inAround, PathNode[] inNeighbors)
        {
            int x = inAround.X;
            int y = inAround.Y;

            if (y > 0)
            {
                inNeighbors[0] = this.mSearchSpace[y - 1, x];
            }
            else
            {
                inNeighbors[0] = null;
            }

            if (x > 0)
            {
                inNeighbors[1] = this.mSearchSpace[y, x - 1];
            }
            else
            {
                inNeighbors[1] = null;
            }

            if (x < this.Width - 1)
            {
                inNeighbors[2] = this.mSearchSpace[y, x + 1];
            }
            else
            {
                inNeighbors[2] = null;
            }

            if (y < this.Height - 1)
            {
                inNeighbors[3] = this.mSearchSpace[y + 1, x];
            }
            else
            {
                inNeighbors[3] = null;
            }
        }

        private LinkedList<PathNode> ReconstructPath(PathNode current_node)
        {
            LinkedList<PathNode> result = new LinkedList<PathNode>();

            this.ReconstructPathRecursive(current_node, result);

            return result;
        }
        private void ReconstructPathRecursive(PathNode current_node, LinkedList<PathNode> result)
        {
            PathNode item = current_node;
            result.AddFirst(item);
            while ((item = item.parent) != null)
            {
                result.AddFirst(item);
            }
        }

        //private class OpenCloseMap
        //{
        //    private PathNode[,] m_Map;
        //    public int Width { get; private set; }
        //    public int Height { get; private set; }
        //    public int Count { get; private set; }

        //    public PathNode this[Int32 x, Int32 y]
        //    {
        //        get
        //        {
        //            return m_Map[x, y];
        //        }
        //    }

        //    public PathNode this[PathNode Node]
        //    {
        //        get
        //        {
        //            return m_Map[Node.X, Node.Y];
        //        }

        //    }

        //    public bool IsEmpty
        //    {
        //        get
        //        {
        //            return Count == 0;
        //        }
        //    }

        //    public OpenCloseMap(int inWidth, int inHeight)
        //    {
        //        m_Map = new PathNode[inWidth, inHeight];
        //        Width = inWidth;
        //        Height = inHeight;
        //    }

        //    public void Add(PathNode inValue)
        //    {
        //        PathNode item = m_Map[inValue.X, inValue.Y];
        //        Count++;
        //        m_Map[inValue.X, inValue.Y] = inValue;
        //    }

        //    public bool Contains(PathNode inValue)
        //    {
        //        PathNode item = m_Map[inValue.X, inValue.Y];

        //        if (item == null)
        //            return false;
        //        return true;
        //    }

        //    public void Remove(PathNode inValue)
        //    {
        //        PathNode item = m_Map[inValue.X, inValue.Y];
        //        Count--;
        //        m_Map[inValue.X, inValue.Y] = null;
        //    }
        //}

        public class PathNode : IPathNode, IComparer<PathNode>, IWeightAddable<double>
        {
            public static readonly PathNode Comparer = new PathNode(0, 0, default(TPathNode));

            public TPathNode UserItem { get; internal set; }
            public double G { get; internal set; }
            public double Optimal { get; internal set; }
            public double F { get; internal set; }

            public PathNode parent;

            public bool IsBlocked(int X, int Y, bool lastTile) => this.UserItem.IsBlocked(X, Y, lastTile);

            public int X { get; internal set; }
            public int Y { get; internal set; }
            public int extraWeight;
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

            public PathNode(int inX, int inY, TPathNode inUserContext)
            {
                this.X = inX;
                this.Y = inY;
                this.UserItem = inUserContext;
            }


            public double WeightChange
            {
                get => this.F;
                set => this.F = value;
            }

            public bool BeenThere;
        }
    }
}
