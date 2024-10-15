namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalogs.Vouchers;
using WibboEmulator.Games.GameClients;

internal sealed class RedeemVoucherEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var voucherCode = packet.PopString().Replace("\r", "");

        if (!VoucherManager.TryGetVoucher(voucherCode, out var voucher))
        {
            Session.SendPacket(new VoucherRedeemErrorComposer(0));
            return;
        }

        if (voucher.CurrentUses >= voucher.MaxUses)
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;

        var haveVoucher = UserVoucherDao.HaveVoucher(dbClient, Session.User.Id, voucherCode);

        if (!haveVoucher)
        {
            return;
        }
        else
        {
            UserVoucherDao.Insert(dbClient, Session.User.Id, voucherCode);
        }

        voucher.UpdateUses();

        if (voucher.Type == VoucherType.Credit)
        {
            Session.User.Credits += voucher.Value;
            Session.SendPacket(new CreditBalanceComposer(Session.User.Credits));
        }
        else if (voucher.Type == VoucherType.Ducket)
        {
            Session.User.Duckets += voucher.Value;
            Session.SendPacket(new ActivityPointNotificationComposer(Session.User.Duckets, voucher.Value));
        }

        //Session.SendPacket(new VoucherRedeemOkComposer());
    }
}
