using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ItemMoodlightDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT enabled,current_preset,preset_one,preset_two,preset_three FROM room_items_moodlight WHERE item_id = '" + ItemId + "' LIMIT 1");
            Row = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO room_items_moodlight (item_id,enabled,current_preset,preset_one,preset_two,preset_three) VALUES ('" + ItemId + "','0','1','#000000,255,0','#000000,255,0','#000000,255,0')");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT enabled,current_preset,preset_one,preset_two,preset_three FROM room_items_moodlight WHERE item_id=" + ItemId + " LIMIT 1");
            Row = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE room_items_moodlight SET enabled = 1 WHERE item_id = '" + this.ItemId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE room_items_moodlight SET enabled = 0 WHERE item_id = '" + this.ItemId + "' LIMIT 1");
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE room_items_moodlight SET preset_" + Pr + " = '@color," + Intensity + "," + ButterflyEnvironment.BoolToEnum(BgOnly) + "' WHERE item_id = '" + this.ItemId + "' LIMIT 1");
            dbClient.AddParameter("color", Color);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO room_items_moodlight (item_id, enabled, current_preset, preset_one, preset_two, preset_three) VALUES (@id, '0', 1, @preset, @preset, @preset)");
            dbClient.AddParameter("id", Item.Id);
            dbClient.AddParameter("preset", "#000000,255,0");
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO room_items_moodlight (item_id, enabled, current_preset, preset_one, preset_two, preset_three)" +
                "SELECT '" + ItemId + "', enabled, current_preset, preset_one, preset_two, preset_three FROM room_items_moodlight WHERE item_id = '" + OldItemId + "'");
        }
    }
}