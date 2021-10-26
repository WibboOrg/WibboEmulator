using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ItemWiredDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO wired_items (trigger_id, trigger_data_2, trigger_data, all_user_triggerable, triggers_item) " +
                                 "SELECT '" + ItemId + "', trigger_data_2, trigger_data, all_user_triggerable, triggers_item FROM wired_items WHERE trigger_id = '" + OldItemId + "'");
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT triggers_item FROM wired_items WHERE trigger_id = '" + id + "' AND triggers_item != ''");
            DataRow wiredRow = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE wired_items SET triggers_item=@triggeritems WHERE trigger_id = '" + id + "' LIMIT 1");
            dbClient.AddParameter("triggeritems", triggerItems);
            dbClient.RunQuery();
        }
    }
}