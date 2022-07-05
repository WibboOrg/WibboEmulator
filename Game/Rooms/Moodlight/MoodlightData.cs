using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;
using System.Text;

namespace WibboEmulator.Game.Rooms.Moodlight
{
    public class MoodlightData
    {
        public int ItemId;
        public int CurrentPreset;
        public bool Enabled;

        public List<MoodlightPreset> Presets;

        public MoodlightData(int ItemId)
        {
            this.ItemId = ItemId;

            DataRow Row = null;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Row = ItemMoodlightDao.GetOne(dbClient, ItemId);
            }

            if (Row == null)
            {
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                ItemMoodlightDao.Insert(dbClient, ItemId);
                Row = ItemMoodlightDao.GetOne(dbClient, ItemId);
            }

            this.Enabled = WibboEnvironment.EnumToBool(Row["enabled"].ToString());
            this.CurrentPreset = Convert.ToInt32(Row["current_preset"]);
            this.Presets = new List<MoodlightPreset>
            {
                GeneratePreset(Convert.ToString(Row["preset_one"])),
                GeneratePreset(Convert.ToString(Row["preset_two"])),
                GeneratePreset(Convert.ToString(Row["preset_three"]))
            };
        }

        public void Enable()
        {
            this.Enabled = true;

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            ItemMoodlightDao.UpdateEnable(dbClient, this.ItemId, 1);
        }

        public void Disable()
        {
            this.Enabled = false;

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            ItemMoodlightDao.UpdateEnable(dbClient, this.ItemId, 0);
        }

        public void UpdatePreset(int Preset, string Color, int Intensity, bool BgOnly, bool Hax = false)
        {
            if (!IsValidColor(Color) || !IsValidIntensity(Intensity) && !Hax)
            {
                return;
            }

            string Pr;

            switch (Preset)
            {
                case 3:

                    Pr = "three";
                    break;

                case 2:

                    Pr = "two";
                    break;

                case 1:
                default:

                    Pr = "one";
                    break;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                ItemMoodlightDao.Update(dbClient, this.ItemId, Color, Pr, Intensity, BgOnly);
            }

            this.GetPreset(Preset).ColorCode = Color;
            this.GetPreset(Preset).ColorIntensity = Intensity;
            this.GetPreset(Preset).BackgroundOnly = BgOnly;
        }

        public static MoodlightPreset GeneratePreset(string Data)
        {
            string[] Bits = Data.Split(',');

            if (!IsValidColor(Bits[0]))
            {
                Bits[0] = "#000000";
            }

            return new MoodlightPreset(Bits[0], int.Parse(Bits[1]), WibboEnvironment.EnumToBool(Bits[2]));
        }

        public MoodlightPreset GetPreset(int i)
        {
            i--;

            if (this.Presets[i] != null)
            {
                return this.Presets[i];
            }

            return new MoodlightPreset("#000000", 255, false);
        }

        public static bool IsValidColor(string ColorCode)
        {
            switch (ColorCode)
            {
                case "#000000":
                case "#0053F7":
                case "#EA4532":
                case "#82F349":
                case "#74F5F5":
                case "#E759DE":
                case "#F2F851":

                    return true;

                default:

                    return false;
            }
        }

        public static bool IsValidIntensity(int Intensity)
        {
            if (Intensity < 0 || Intensity > 255)
            {
                return false;
            }

            return true;
        }

        public string GenerateExtraData()
        {
            MoodlightPreset Preset = this.GetPreset(this.CurrentPreset);
            StringBuilder SB = new StringBuilder();

            SB.Append(this.Enabled == true ? 2 : 1);

            SB.Append(",");
            SB.Append(this.CurrentPreset);
            SB.Append(",");

            SB.Append(Preset.BackgroundOnly == true ? 2 : 1);

            SB.Append(",");
            SB.Append(Preset.ColorCode);
            SB.Append(",");
            SB.Append(Preset.ColorIntensity);
            return SB.ToString();
        }
    }
}
