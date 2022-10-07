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
            haveVoucher = UserVoucherDao.HaveVoucher(dbClient, session.GetUser().Id, voucherCode);
        }

        if (!haveVoucher)
        {
            return;
        }
        else
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserVoucherDao.Insert(dbClient, session.GetUser().Id, voucherCode);
        }

        voucher.UpdateUses();

        if (voucher.Type == VoucherType.CREDIT)
        {
            session.GetUser().Credits += voucher.Value;
            session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));
        }
        else if (voucher.Type == VoucherType.DUCKET)
        {
            session.GetUser().Duckets += voucher.Value;
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().Duckets, voucher.Value));
        }

        //session.SendPacket(new VoucherRedeemOkComposer());
    }
}
