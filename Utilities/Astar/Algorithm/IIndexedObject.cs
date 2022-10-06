namespace WibboEmulator.Utilities.Astar.Algorithm;

public interface IWeightAlterable<T>
{
    T Weight { get; set; }
}
