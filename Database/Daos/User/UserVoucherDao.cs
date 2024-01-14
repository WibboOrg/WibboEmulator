namespace WibboEmulator.Database.Daos.User;

using System.Data;
using Dapper;

internal sealed class UserVoucherDao
{
    internal static bool HaveVoucher(IDbConnection dbClient, int userId, string voucherCode) => dbClient.QueryFirstOrDefault<int>(
        "SELECT id FROM user_voucher WHERE user_id = @UserId AND voucher = @Voucher LIMIT 1",
        new { UserId = userId, Voucher = voucherCode }) > 0;

    internal static void Insert(IDbConnection dbClient, int userId, string voucherCode) => dbClient.Execute(
        "INSERT INTO user_voucher (user_id, voucher) VALUES (@UserId, @Voucher)",
        new { UserId = userId, Voucher = voucherCode });
}
