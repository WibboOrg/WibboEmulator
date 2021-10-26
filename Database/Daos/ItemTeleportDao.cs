using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class ItemTeleportDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO tele_links (tele_one_id, tele_two_id) VALUES (" + Item1Id + ", " + Item2Id + "), (" + Item2Id + ", " + Item1Id + ")");
            dbClient.RunQuery();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT tele_two_id FROM tele_links WHERE tele_one_id = '" + TeleId + "'");
            DataRow row = dbClient.GetRow();
        }

        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT tele_two_id FROM tele_links WHERE tele_one_id = '" + oldId + "'");
            DataRow rowTele = dbClient.GetRow();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("INSERT INTO tele_links (tele_one_id, tele_two_id) VALUES ('" + newId + "', '" + newIdTwo + "');");
        }
    }
}