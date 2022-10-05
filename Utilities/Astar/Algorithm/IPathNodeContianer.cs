namespace Astar.Algorithm;

public interface IPathNodeContianer
{
    IPathNode GetPathNode(int y, int x);
    bool IsBlocked(int y, int x);
    int GetLength(int dimenstion);

}
