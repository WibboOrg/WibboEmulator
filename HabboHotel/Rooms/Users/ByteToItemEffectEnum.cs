namespace Butterfly.HabboHotel.Rooms
{
    public enum ItemEffectType
    {
        NONE,
        SWIM,
        SWIMLOW,
        SWIMHALLOWEEN,
        ICESKATES,
        NORMALSKATES,
        PUBLICPOOL,
        TRAMPOLINE,
        TREADMILL,
        CROSSTRAINER,
    }

    public static class ByteToItemEffectEnum
    {
        public static ItemEffectType Parse(byte pByte)
        {
            switch (pByte)
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
}
