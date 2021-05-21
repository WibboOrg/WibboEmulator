namespace Butterfly.HabboHotel.Rooms
{

    public enum ItemEffectType
    {
        None,
        Swim,
        SwimLow,
        SwimHalloween,
        Iceskates,
        Normalskates,
        PublicPool,
        Trampoline,
        TreadMill,
        CrossTrainer,
    }

    public static class ByteToItemEffectEnum
    {
        public static ItemEffectType Parse(byte pByte)
        {
            switch (pByte)
            {
                case 0:
                    return ItemEffectType.None;
                case 1:
                    return ItemEffectType.Swim;
                case 2:
                    return ItemEffectType.Normalskates;
                case 3:
                    return ItemEffectType.Iceskates;
                case 4:
                    return ItemEffectType.SwimLow;
                case 5:
                    return ItemEffectType.SwimHalloween;
                case 6:
                    return ItemEffectType.PublicPool;
                case 7:
                    return ItemEffectType.Trampoline;
                case 8:
                    return ItemEffectType.TreadMill;
                case 9:
                    return ItemEffectType.CrossTrainer;
                default:
                    return ItemEffectType.None;
            }
        }
    }
}
