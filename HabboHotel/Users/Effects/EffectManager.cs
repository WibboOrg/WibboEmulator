using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Users.Effect
{
    public class EffectManager
    {
        private readonly List<int> _effects;
        private readonly List<int> _effectsStaff;

        public List<int> GetEffects()
        {
            return this._effects;
        }

        public EffectManager()
        {
            this._effects = new List<int>();
            this._effectsStaff = new List<int>();
        }

        public void Init()
        {
            this._effects.Clear();
            this._effectsStaff.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id, only_staff FROM systeme_effects ORDER by id ASC");
                DataTable table = dbClient.GetTable();
                if (table == null)
                {
                    return;
                }

                foreach (DataRow dataRow in table.Rows)
                {
                    int EffectId = Convert.ToInt32(dataRow["id"]);

                    if (ButterflyEnvironment.EnumToBool((string)dataRow["only_staff"]))
                    {
                        if (!this._effectsStaff.Contains(EffectId))
                        {
                            this._effectsStaff.Add(EffectId);
                        }
                    }
                    else
                    {
                        if (!this._effects.Contains(EffectId))
                        {
                            this._effects.Add(EffectId);
                        }
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
