using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class LogChatPubDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO chatlogs_pub (user_id, user_name, timestamp, message) VALUES ('" + this.GetHabbo().Id + "', @pseudo, UNIX_TIMESTAMP(), @message)");
            dbClient.AddParameter("message", "A vérifié: " + type + Message);
            dbClient.AddParameter("pseudo", this.GetHabbo().Username);
            dbClient.RunQuery();
        }
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO chatlogs_pub (user_id,user_name,timestamp,message) VALUES ('" + this.GetHabbo().Id + "',@pseudo,UNIX_TIMESTAMP(),@message)");
            dbClient.AddParameter("message", "Pub numero " + PubCount + ": " + type + Message);
            dbClient.AddParameter("pseudo", this.GetHabbo().Username);
            dbClient.RunQuery();
        }
    }
}