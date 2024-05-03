namespace WibboEmulator.Games.Groups;

public class GroupBadgeParts(int id, string assetOne, string assetTwo)
{
    public int Id { get; private set; } = id;
    public string AssetOne { get; private set; } = assetOne;
    public string AssetTwo { get; private set; } = assetTwo;
}
