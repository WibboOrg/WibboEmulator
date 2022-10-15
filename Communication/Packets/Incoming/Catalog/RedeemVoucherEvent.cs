namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalog.Vouchers;
using WibboEmulator.Games.GameClients;

internal class RedeemVoucherEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var voucherCode = packet.PopString().Replace("\r", "");

        if (!WibboEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(voucherCode, out var voucher))
        {
            session.SendPacket(new VoucherRedeemErrorComposer(0));
            return;
        }

        if (voucher.CurrentUses >= voucher.MaxUses)
        {
            return;
        }

        var haveVoucher = false;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            haveVoucher = UserVoucherDao.HaveVoucher(dbClient, session.User.Id, voucherCode);
        }

        if (!haveVoucher)
        {
            return;
        }
        else
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserVoucherDao.Insert(dbClient, session.User.Id, voucherCode);
        }

        voucher.UpdateUses();

        if (voucher.Type == VoucherType.Credit)
        {
            session.User.Credits += voucher.Value;
            session.SendPacket(new CreditBalanceComposer(session.User.Credits));
        }
        else if (voucher.Type == VoucherType.Ducket)
        {
            session.User.Duckets += voucher.Value;
            session.SendPacket(new ActivityPointNotificationComposer(session.User.Duckets, voucher.Value));
        }

        //session.SendPacket(new VoucherRedeemOkComposer());
    }
}
