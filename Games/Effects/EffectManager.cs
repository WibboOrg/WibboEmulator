namespace WibboEmulator.Games.Effects;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public class EffectManager
{
    private Dictionary<int, bool> Effects { get; }

    public EffectManager() => this.Effects = new Dictionary<int, bool>();

    public void Initialize(IDbConnection dbClient)
    {
        this.Effects.Clear();

        var emulatorEffectList = EmulatorEffectDao.GetAll(dbClient);
        if (emulatorEffectList.Count == 0)
        {
            return;
        }

        foreach (var emulatorEffect in emulatorEffectList)
        {
            var effectId = emulatorEffect.Id;

            if (!this.Effects.ContainsKey(effectId))
            {
                this.Effects.Add(effectId, emulatorEffect.OnlyStaff);
            }
        }
    }

    public bool HasEffect(int effectId, bool isStaff = false) => this.Effects.TryGetValue(effectId, out var onlyStaff) && (!onlyStaff || isStaff);
}
