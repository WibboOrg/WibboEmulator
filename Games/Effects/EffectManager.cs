namespace WibboEmulator.Games.Effects;
using System.Data;
using WibboEmulator.Database.Daos.Emulator;
using WibboEmulator.Database.Interfaces;

public class EffectManager
{
    private readonly List<int> _effectsStaff;

    public List<int> Effects { get; }

    public EffectManager()
    {
        this.Effects = new List<int>();
        this._effectsStaff = new List<int>();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this.Effects.Clear();
        this._effectsStaff.Clear();

        var table = EmulatorEffectDao.GetAll(dbClient);
        if (table == null)
        {
            return;
        }

        foreach (DataRow dataRow in table.Rows)
        {
            var effectId = Convert.ToInt32(dataRow["id"]);

            if (WibboEnvironment.EnumToBool((string)dataRow["only_staff"]))
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
