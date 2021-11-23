using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Database.Daos
{
    class ItemWiredDao
    {
        internal static void InsertDuplicate(IQueryAdapter dbClient, int itemId, int oldItemId)
        {
            dbClient.RunQuery("INSERT INTO `item_wired` (trigger_id, trigger_data_2, trigger_data, all_user_triggerable, triggers_item, delay) " +
                                 "SELECT '" + itemId + "', trigger_data_2, trigger_data, all_user_triggerable, triggers_item, delay FROM `item_wired` WHERE trigger_id = '" + oldItemId + "'");
        }

        internal static DataRow GetOne(IQueryAdapter dbClient, int triggerId)
        {
            dbClient.SetQuery("SELECT trigger_data, trigger_data_2, triggers_item, all_user_triggerable, delay FROM `item_wired` WHERE trigger_id = @id");
            dbClient.AddParameter("id", triggerId);

            return dbClient.GetRow();
        }

        internal static void UpdateTriggerItem(IQueryAdapter dbClient, string triggerItems, int triggerId)
        {
            dbClient.SetQuery("UPDATE `item_wired` SET triggers_item = @triggeritems WHERE trigger_id = '" + triggerId + "' LIMIT 1");
            dbClient.AddParameter("triggeritems", triggerItems);
            dbClient.RunQuery();
        }

        internal static void Delete(IQueryAdapter dbClient, int triggerId)
        {
            dbClient.RunQuery("DELETE FROM `item_wired` WHERE trigger_id = '" + triggerId + "'");
        }

        internal static void Insert(IQueryAdapter dbClient, int triggerId, string triggerData, string triggerData2, bool allUsertriggerable, string triggersitem, int delay)
        {
            dbClient.SetQuery("INSERT INTO `item_wired` (trigger_id, trigger_data, trigger_data_2, all_user_triggerable, triggers_item, delay) VALUES (@id, @trigger_data, @trigger_data_2, @triggerable, @triggers_item, @delay)");
            dbClient.AddParameter("id", triggerId);
            dbClient.AddParameter("trigger_data", triggerData);
            dbClient.AddParameter("trigger_data_2", triggerData2);
            dbClient.AddParameter("triggerable", (allUsertriggerable ? 1 : 0));
            dbClient.AddParameter("triggers_item", triggersitem);
            dbClient.AddParameter("delay", delay);
            dbClient.RunQuery();
        }
    }
}