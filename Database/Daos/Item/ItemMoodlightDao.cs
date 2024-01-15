namespace WibboEmulator.Database.Daos.Item;
using System.Data;
using Dapper;

internal sealed class ItemMoodlightDao
{
    internal static ItemMoodlightEntity GetOne(IDbConnection dbClient, int itemId) => dbClient.QuerySingleOrDefault<ItemMoodlightEntity>(
        "SELECT enabled, current_preset, preset_one, preset_two, preset_three FROM `item_moodlight` WHERE item_id = '" + itemId + "' LIMIT 1");

    internal static void UpdateEnabled(IDbConnection dbClient, int itemId, bool enabled) => dbClient.Execute(
        "UPDATE `item_moodlight` SET enabled = @Enabled WHERE item_id = @ItemId LIMIT 1",
        new { Enabled = enabled, ItemId = itemId });

    internal static void Update(IDbConnection dbClient, int itemId, string color, string pr, int intensity, bool bgOnly, int currentPreset) => dbClient.Execute(
        "UPDATE item_moodlight SET preset_" + pr + " = @Preset, current_preset = @CurrentPreset WHERE item_id = @ItemId LIMIT 1",
        new { Preset = color + "," + intensity + "," + (bgOnly ? 1 : 0), CurrentPreset = currentPreset, ItemId = itemId });

    internal static void Insert(IDbConnection dbClient, int itemId) => dbClient.Execute(
        "INSERT INTO item_moodlight (item_id, enabled, current_preset, preset_one, preset_two, preset_three) VALUES (@Id, 0, 1, @Preset, @Preset, @Preset)",
        new { Id = itemId, Preset = "#000000,255,0" });

    internal static void InsertDuplicate(IDbConnection dbClient, int itemId, int oldItemId) => dbClient.Execute(
        "INSERT INTO `item_moodlight` (item_id, enabled, current_preset, preset_one, preset_two, preset_three)" +
        "SELECT '" + itemId + "', enabled, current_preset, preset_one, preset_two, preset_three FROM `item_moodlight` WHERE item_id = '" + oldItemId + "'");
}

public class ItemMoodlightEntity
{
    public int ItemId { get; set; }
    public bool Enabled { get; set; }
    public int CurrentPreset { get; set; }
    public string PresetOne { get; set; }
    public string PresetTwo { get; set; }
    public string PresetThree { get; set; }
}