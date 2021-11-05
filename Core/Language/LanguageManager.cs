using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Core
{
    public enum Language
    {
        NONE,
        FRANCAIS,
        ANGLAIS,
        PORTUGAIS,
    }

    public class LanguageManager
    {
        private Dictionary<string, string> _valuesFr;
        private Dictionary<string, string> _valuesEn;
        private Dictionary<string, string> _valuesBr;

        public void Init()
        {
            this._valuesFr = new Dictionary<string, string>();
            this._valuesEn = new Dictionary<string, string>();
            this._valuesBr = new Dictionary<string, string>();
            this.InitLocalValues();
        }

        public void InitLocalValues()
        {
            this._valuesFr.Clear();
            this._valuesEn.Clear();
            this._valuesBr.Clear();

            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                table = EmulatorTextDao.GetAll(dbClient);
                
            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                string key = (string)dataRow["identifiant"];
                string value_fr = (string)dataRow["value_fr"];
                string value_en = (string)dataRow["value_en"];
                string value_br = (string)dataRow["value_br"];

                this._valuesFr.Add(key, value_fr);
                this._valuesEn.Add(key, value_en);
                this._valuesBr.Add(key, value_br);
            }
        }

        public string TryGetValue(string value, Language Language)
        {
            if (Language == Language.FRANCAIS)
            {
                return this._valuesFr.ContainsKey(value) ? this._valuesFr[value] : "Pas de language locale trouver pour [" + value + "] (fr)";
            }
            else if (Language == Language.ANGLAIS)
            {
                return this._valuesEn.ContainsKey(value) ? this._valuesEn[value] : "Pas de language locale trouver pour [" + value + "] (en)";
            }
            else if (Language == Language.PORTUGAIS)
            {
                return this._valuesBr.ContainsKey(value) ? this._valuesBr[value] : "Pas de language locale trouver pour [" + value + "] (br)";
            }
            else
            {
                return this._valuesFr.ContainsKey(value) ? this._valuesFr[value] : "Pas de language locale trouver pour [" + value + "] (def)";
            }
        }

        public static Language ParseLanguage(string country)
        {
            switch (country)
            {
                case "fr":
                    return Language.FRANCAIS;
                case "en":
                    return Language.ANGLAIS;
                case "br":
                    return Language.PORTUGAIS;
                default:
                    return Language.FRANCAIS;
            }
        }
    }
}