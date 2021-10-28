using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ItemWiredDao
    {
        internal static void InsertDuplicate(IQueryAdapter dbClient, int itemId, int oldItemId)
        {
            dbClient.RunQuery("INSERT INTO wired_items (trigger_id, trigger_data_2, trigger_data, all_user_triggerable, triggers_item) " +
                                 "SELECT '" + itemId + "', trigger_data_2, trigger_data, all_user_triggerable, triggers_item FROM wired_items WHERE trigger_id = '" + oldItemId + "'");
        }

        internal static DataRow GetOne(IQueryAdapter dbClient, int triggerId)
        {
            dbClient.SetQuery("SELECT triggers_item FROM wired_items WHERE trigger_id = '" + triggerId + "' AND triggers_item != ''");
            return dbClient.GetRow();
        }

        internal static void UpdateTriggerItem(IQueryAdapter dbClient, string triggerItems, int triggerId)
        {
            dbClient.SetQuery("UPDATE wired_items SET triggers_item=@triggeritems WHERE trigger_id = '" + triggerId + "' LIMIT 1");
            dbClient.AddParameter("triggeritems", triggerItems);
            dbClient.RunQuery();
        }
    }
}