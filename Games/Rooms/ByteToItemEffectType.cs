namespace WibboEmulator.Games.Rooms;

public static class ByteToItemEffectType
{
    public static ItemEffectType Parse(byte number)
    {
        switch (number)
        {
            case 0:
                return ItemEffectType.NONE;
            case 1:
                return ItemEffectType.SWIM;
            case 2:
                return ItemEffectType.NORMALSKATES;
            case 3:
                return ItemEffectType.ICESKATES;
            case 4:
                return ItemEffectType.SWIMLOW;
            case 5:
                return ItemEffectType.SWIMHALLOWEEN;
            case 6:
                return ItemEffectType.PUBLICPOOL;
            case 7:
                return ItemEffectType.TRAMPOLINE;
            case 8:
                return ItemEffectType.TREADMILL;
            case 9:
                return ItemEffectType.CROSSTRAINER;
            default:
                return ItemEffectType.NONE;
        }
    }
}
