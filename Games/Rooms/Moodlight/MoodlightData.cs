namespace WibboEmulator.Games.Rooms.Moodlight;
using System.Text;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Item;

public class MoodlightData(int itemId, bool enabled, int currentPreset, string presetOne, string presetTwo, string presetThree)
{
    public int ItemId { get; set; } = itemId;
    public int CurrentPreset { get; set; } = currentPreset;
    public bool Enabled { get; set; } = enabled;

    public List<MoodlightPreset> Presets { get; set; } =
        [
            GeneratePreset(presetOne),
            GeneratePreset(presetTwo),
            GeneratePreset(presetThree)
        ];

    public void Enable() => this.Enabled = true;

    public void Disable() => this.Enabled = false;

    public void UpdatePreset(int preset, string color, int intensity, bool bgOnly, bool inDb = true)
    {
        if (!IsValidColor(color) || !IsValidIntensity(intensity))
        {
            return;
        }

        var pr = preset switch
        {
            3 => "three",
            2 => "two",
            _ => "one",
        };

        if (inDb)
        {
            using var dbClient = DatabaseManager.Connection;
            ItemMoodlightDao.Update(dbClient, this.ItemId, color, pr, intensity, bgOnly, preset);
        }

        this.GetPreset(preset).ColorCode = color;
        this.GetPreset(preset).ColorIntensity = intensity;
        this.GetPreset(preset).BackgroundOnly = bgOnly;
    }

    public static MoodlightPreset GeneratePreset(string data)
    {
        if (!data.Contains(','))
        {
            return new MoodlightPreset("#000000", 0, false);
        }

        var bits = data.Split(',');

        if (bits.Length != 3)
        {
            return new MoodlightPreset("#000000", 0, false);
        }

        if (!IsValidColor(bits[0]))
        {
            bits[0] = "#000000";
        }

        return new MoodlightPreset(bits[0], int.Parse(bits[1]), bits[2] == "1");
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
        var sb = new StringBuilder()
        .Append(this.Enabled ? 2 : 1)
        .Append(',')
        .Append(this.CurrentPreset)
        .Append(',')
        .Append(preset.BackgroundOnly ? 2 : 1)
        .Append(',')
        .Append(preset.ColorCode)
        .Append(',')
        .Append(preset.ColorIntensity);

        return sb.ToString();
    }
}
