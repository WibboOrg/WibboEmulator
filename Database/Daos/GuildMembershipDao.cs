using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class GuildMembershipDao
    {
        internal static void deleteGuild(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("UPDATE `catalog_items` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
            dbClient.AddParameter("limitSells", Item.LimitedEditionSells);
            dbClient.AddParameter("itemId", Item.Id);
            dbClient.RunQuery();
        }
    }
}
