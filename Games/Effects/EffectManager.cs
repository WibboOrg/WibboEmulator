namespace WibboEmulator.Games.Effects;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public static class EffectManager
{
    private static Dictionary<int, bool> Effects { get; } = new();

    public static void Initialize(IDbConnection dbClient)
    {
        Effects.Clear();

        var emulatorEffectList = EmulatorEffectDao.GetAll(dbClient);

        foreach (var emulatorEffect in emulatorEffectList)
        {
            var effectId = emulatorEffect.Id;

            if (!Effects.ContainsKey(effectId))
            {
                Effects.Add(effectId, emulatorEffect.OnlyStaff);
            }
        }
    }

    public static bool HasEffect(int effectId, bool isStaff = false) => Effects.TryGetValue(effectId, out var onlyStaff) && (!onlyStaff || isStaff);
}
