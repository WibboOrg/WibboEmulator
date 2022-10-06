namespace WibboEmulator.Utilities.Astar.Algorithm;

public interface IWeightAddable<T>
{
    T WeightChange { get; set; }
}
