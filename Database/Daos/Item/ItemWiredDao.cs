namespace WibboEmulator.Database.Daos.Item;
using System.Data;
using Dapper;

internal sealed class ItemWiredDao
{
    internal static void InsertDuplicate(IDbConnection dbClient, int itemId, int oldItemId) => dbClient.Execute(
        @"INSERT INTO `item_wired` (trigger_id, trigger_data_2, trigger_data, all_user_triggerable, triggers_item, delay) 
        SELECT '" + itemId + "', trigger_data_2, trigger_data, all_user_triggerable, triggers_item, delay FROM `item_wired` WHERE trigger_id = '" + oldItemId + "'");

    internal static ItemWiredEntity GetOne(IDbConnection dbClient, int triggerId) => dbClient.QuerySingleOrDefault<ItemWiredEntity>(
        "SELECT trigger_data, trigger_data_2, triggers_item, all_user_triggerable, delay FROM `item_wired` WHERE trigger_id = @Id",
        new { Id = triggerId });

    internal static void UpdateTriggerItem(IDbConnection dbClient, string triggerItems, int triggerId) => dbClient.Execute(
        "UPDATE item_wired SET triggers_item = @TriggerItems WHERE trigger_id = @TriggerId LIMIT 1",
        new { TriggerItems = triggerItems, TriggerId = triggerId });

    internal static void Replace(IDbConnection dbClient, int triggerId, string triggerData, string triggerData2, bool allUserTriggerable, string triggersItem, int delay) => dbClient.Execute(
        "REPLACE INTO item_wired (trigger_id, trigger_data, trigger_data_2, all_user_triggerable, triggers_item, delay) VALUES (@Id, @TriggerData, @TriggerData2, @AllUserTriggerable, @TriggersItem, @Delay)",
        new { Id = triggerId, TriggerData = triggerData, TriggerData2 = triggerData2, AllUserTriggerable = allUserTriggerable ? 1 : 0, TriggersItem = triggersItem, Delay = delay });
}

public class ItemWiredEntity
{
    public int TriggerId { get; set; }
    public string TriggerData2 { get; set; }
    public string TriggerData { get; set; }
    public bool AllUserTriggerable { get; set; }
    public string TriggersItem { get; set; }
    public int Delay { get; set; }
}