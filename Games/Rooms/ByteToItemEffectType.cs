namespace WibboEmulator.Games.Rooms;

public static class ByteToItemEffectType
{
    public static ItemEffectType Parse(byte number) => number switch
    {
        0 => ItemEffectType.NONE,
        1 => ItemEffectType.SWIM,
        2 => ItemEffectType.NORMALSKATES,
        3 => ItemEffectType.ICESKATES,
        4 => ItemEffectType.SWIMLOW,
        5 => ItemEffectType.SWIMHALLOWEEN,
        6 => ItemEffectType.PUBLICPOOL,
        7 => ItemEffectType.TRAMPOLINE,
        8 => ItemEffectType.TREADMILL,
        9 => ItemEffectType.CROSSTRAINER,
        _ => ItemEffectType.NONE,
    };
}
