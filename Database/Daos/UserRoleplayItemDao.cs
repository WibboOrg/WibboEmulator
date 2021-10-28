using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserRoleplayItemDao
    {
        internal static void Delete(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_rpitems WHERE user_id = '" + this._id + "' AND rp_id = '" + this._rpId + "'");
        }

        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM user_rpitems WHERE user_id = '" + this._id + "' AND rp_id = '" + this._rpId + "'");
            dbClient.AddParameter("userid", this._id);
            return dbClient.GetTable();
        }

        internal static void Insert(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO user_rpitems (user_id, rp_id, item_id, count) VALUES ('" + this._id + "', '" + this._rpId + "', '" + pItemId + "', '" + pCount + "')");
        }

        internal static void Update(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_rpitems SET count = count + '" + pCount + "' WHERE id = '" + Item.Id + "' LIMIT 1");
        }
        
        internal static void Update(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE user_rpitems SET count = count - '" + Count + "' WHERE id = '" + Item.Id + "' LIMIT 1");
        }

        internal static void Delete(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM user_rpitems WHERE id = '" + Item.Id + "' LIMIT 1");
        }
    }
}