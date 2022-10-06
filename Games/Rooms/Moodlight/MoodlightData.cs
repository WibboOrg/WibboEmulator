namespace WibboEmulator.Games.Rooms.Moodlight;
using System.Data;
using System.Text;
using WibboEmulator.Database.Daos.Item;

public class MoodlightData
{
    public int ItemId { get; set; }
    public int CurrentPreset { get; set; }
    public bool Enabled { get; set; }

    public List<MoodlightPreset> Presets { get; set; }

    public MoodlightData(int itemId)
    {
        this.ItemId = itemId;

        DataRow row = null;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            row = ItemMoodlightDao.GetOne(dbClient, itemId);
        }

        if (row == null)
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            ItemMoodlightDao.Insert(dbClient, itemId);
            row = ItemMoodlightDao.GetOne(dbClient, itemId);
        }

        this.Enabled = WibboEnvironment.EnumToBool(row["enabled"].ToString());
        this.CurrentPreset = Convert.ToInt32(row["current_preset"]);
        this.Presets = new List<MoodlightPreset>
        {
            GeneratePreset(Convert.ToString(row["preset_one"])),
            GeneratePreset(Convert.ToString(row["preset_two"])),
            GeneratePreset(Convert.ToString(row["preset_three"]))
        };
    }

    public void Enable()
    {
        this.Enabled = true;

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ItemMoodlightDao.UpdateEnable(dbClient, this.ItemId, 1);
    }

    public void Disable()
    {
        this.Enabled = false;

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        ItemMoodlightDao.UpdateEnable(dbClient, this.ItemId, 0);
    }

    public void UpdatePreset(int preset, string color, int intensity, bool bgOnly, bool hax = false)
    {
        if (!IsValidColor(color) || (!IsValidIntensity(intensity) && !hax))
        {
            return;
        }

        var pr = preset switch
        {
            3 => "three",
            2 => "two",
            _ => "one",
        };
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            ItemMoodlightDao.Update(dbClient, this.ItemId, color, pr, intensity, bgOnly);
        }

        this.GetPreset(preset).ColorCode = color;
        this.GetPreset(preset).ColorIntensity = intensity;
        this.GetPreset(preset).BackgroundOnly = bgOnly;
    }

    public static MoodlightPreset GeneratePreset(string data)
    {
        var bits = data.Split(',');

        if (!IsValidColor(bits[0]))
        {
            bits[0] = "#000000";
        }

        return new MoodlightPreset(bits[0], int.Parse(bits[1]), WibboEnvironment.EnumToBool(bits[2]));
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

    public static bool IsValidColor(string colorCode) => colorCode switch
    {
        "#000000" or "#0053F7" or "#EA4532" or "#82F349" or "#74F5F5" or "#E759DE" or "#F2F851" => true,
        _ => false,
    };

    public static bool IsValidIntensity(int intensity)
    {
        if (intensity is < 0 or > 255)
        {
            return false;
        }

        return true;
    }

    public string GenerateExtraData()
    {
        var preset = this.GetPreset(this.CurrentPreset);
        var sb = new StringBuilder();

        _ = sb.Append(this.Enabled ? 2 : 1);

        _ = sb.Append(',');
        _ = sb.Append(this.CurrentPreset);
        _ = sb.Append(',');

        _ = sb.Append(preset.BackgroundOnly ? 2 : 1);

        _ = sb.Append(',');
        _ = sb.Append(preset.ColorCode);
        _ = sb.Append(',');
        _ = sb.Append(preset.ColorIntensity);
        return sb.ToString();
    }
}
