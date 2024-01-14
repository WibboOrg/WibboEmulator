namespace WibboEmulator.Games.Effects;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;

public class EffectManager
{
    private readonly List<int> _effectsStaff;

    public List<int> Effects { get; }

    public EffectManager()
    {
        this.Effects = new List<int>();
        this._effectsStaff = new List<int>();
    }

    public void Init(IDbConnection dbClient)
    {
        this.Effects.Clear();
        this._effectsStaff.Clear();

        var emulatorEffectList = EmulatorEffectDao.GetAll(dbClient);
        if (emulatorEffectList.Count == 0)
        {
            return;
        }

        foreach (var emulatorEffect in emulatorEffectList)
        {
            var effectId = emulatorEffect.Id;

            if (emulatorEffect.OnlyStaff)
            {
                if (!this._effectsStaff.Contains(effectId))
                {
                    this._effectsStaff.Add(effectId);
                }
            }
            else
            {
                if (!this.Effects.Contains(effectId))
                {
                    this.Effects.Add(effectId);
                }
            }
        }
    }

    public bool HaveEffect(int effectId, bool isStaff = false)
    {
        if (this.Effects.Contains(effectId))
        {
            return true;
        }

        if (isStaff && this._effectsStaff.Contains(effectId))
        {
            return true;
        }

        return false;
    }
}
