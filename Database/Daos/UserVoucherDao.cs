namespace Butterfly.Database.Daos
{
    class UserVoucherDao
    {
        internal static bool haveVoucher(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT * FROM `user_voucher` WHERE `user_id` = @userId AND `voucher` = @Voucher LIMIT 1");
            dbClient.AddParameter("userId", Session.GetHabbo().Id);
            dbClient.AddParameter("Voucher", VoucherCode);
            return dbClient.GetRow();
        }

        internal static bool insertVoucher(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("INSERT INTO `user_voucher` (`user_id`,`voucher`) VALUES (@userId, @Voucher)");
            dbClient.AddParameter("userId", Session.GetHabbo().Id);
            dbClient.AddParameter("Voucher", VoucherCode);
            dbClient.RunQuery();
        }
    }
}
