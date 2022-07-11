using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Database.Daos
{
    class UserVoucherDao
    {
        internal static bool HaveVoucher(IQueryAdapter dbClient, int userId, string voucherCode)
        {
            dbClient.SetQuery("SELECT null FROM `user_voucher` WHERE `user_id` = @userId AND `voucher` = @Voucher LIMIT 1");
            dbClient.AddParameter("userId", userId);
            dbClient.AddParameter("Voucher", voucherCode);
            return dbClient.FindsResult();
        }

        internal static void Insert(IQueryAdapter dbClient, int userId, string voucherCode)
        {
            dbClient.SetQuery("INSERT INTO `user_voucher` (`user_id`, `voucher`) VALUES (@userId, @Voucher)");
            dbClient.AddParameter("userId", userId);
            dbClient.AddParameter("Voucher", voucherCode);
            dbClient.RunQuery();
        }
    }
}
