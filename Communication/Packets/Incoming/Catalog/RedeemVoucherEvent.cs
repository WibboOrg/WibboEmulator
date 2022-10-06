namespace WibboEmulator.Communication.Packets.Incoming.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Catalog.Vouchers;
using WibboEmulator.Games.GameClients;

internal class RedeemVoucherEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var VoucherCode = Packet.PopString().Replace("\r", "");

        if (!WibboEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(VoucherCode, out var Voucher))
        {
            session.SendPacket(new VoucherRedeemErrorComposer(0));
            return;
        }

        if (Voucher.CurrentUses >= Voucher.MaxUses)
        {
            return;
        }

        var haveVoucher = false;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            haveVoucher = UserVoucherDao.HaveVoucher(dbClient, session.GetUser().Id, VoucherCode);
        }

        if (!haveVoucher)
        {
            return;
        }
        else
        {
            using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserVoucherDao.Insert(dbClient, session.GetUser().Id, VoucherCode);
        }

        Voucher.UpdateUses();

        if (Voucher.Type == VoucherType.CREDIT)
        {
            session.GetUser().Credits += Voucher.Value;
            session.SendPacket(new CreditBalanceComposer(session.GetUser().Credits));
        }
        else if (Voucher.Type == VoucherType.DUCKET)
        {
            session.GetUser().Duckets += Voucher.Value;
            session.SendPacket(new ActivityPointNotificationComposer(session.GetUser().Duckets, Voucher.Value));
        }

        //session.SendPacket(new VoucherRedeemOkComposer());
    }
}
