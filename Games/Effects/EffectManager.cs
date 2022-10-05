using System.Data;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Games.Effects
{
    public class EffectManager
    {
        private readonly List<int> _effects;
        private readonly List<int> _effectsStaff;

        public List<int> GetEffects() => this._effects;

        public EffectManager()
        {
            this._effects = new List<int>();
            this._effectsStaff = new List<int>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._effects.Clear();
            this._effectsStaff.Clear();

            DataTable table = EmulatorEffectDao.GetAll(dbClient);
            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                int effectId = Convert.ToInt32(dataRow["id"]);

                if (WibboEnvironment.EnumToBool((string)dataRow["only_staff"]))
                {
                    if (!this._effectsStaff.Contains(effectId))
                    {
                        this._effectsStaff.Add(effectId);
                    }
                }
                else
                {
                    if (!this._effects.Contains(effectId))
                    {
                        this._effects.Add(effectId);
                    }
                }
            }
        }

        public bool HaveEffect(int EffectId, bool Staff = false)
        {
            if (this._effects.Contains(EffectId))
            {
                return true;
            }

            if (Staff && this._effectsStaff.Contains(EffectId))
            {
                return true;
            }

            return false;
        }
    }
}
