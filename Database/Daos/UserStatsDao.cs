namespace Butterfly.Database.Daos
{
    class UserStatsDao
    {
        internal static bool haveVoucher(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM `user_voucher` WHERE `user_id` = @userId AND `voucher` = @Voucher LIMIT 1");
            dbClient.AddParameter("userId", Session.GetHabbo().Id);
            dbClient.AddParameter("Voucher", VoucherCode);
            return dbClient.GetRow();
        }
    }
}
