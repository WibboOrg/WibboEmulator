using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Catalog.Vouchers;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RedeemVoucherEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string VoucherCode = Packet.PopString().Replace("\r", "");

            if (!WibboEnvironment.GetGame().GetCatalog().GetVoucherManager().TryGetVoucher(VoucherCode, out Voucher Voucher))
            {
                Session.SendPacket(new VoucherRedeemErrorComposer(0));
                return;
            }

            if (Voucher.CurrentUses >= Voucher.MaxUses)
            {
                return;
            }

            bool haveVoucher = false;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                haveVoucher = UserVoucherDao.HaveVoucher(dbClient, Session.GetUser().Id, VoucherCode);

            if (!haveVoucher)
            {
                return;
            }
            else
            {
                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserVoucherDao.Insert(dbClient, Session.GetUser().Id, VoucherCode);
            }

            Voucher.UpdateUses();

            if (Voucher.Type == VoucherType.CREDIT)
            {
                Session.GetUser().Credits += Voucher.Value;
                Session.SendPacket(new CreditBalanceComposer(Session.GetUser().Credits));
            }
            else if (Voucher.Type == VoucherType.DUCKET)
            {
                Session.GetUser().Duckets += Voucher.Value;
                Session.SendPacket(new ActivityPointNotificationComposer(Session.GetUser().Duckets, Voucher.Value));
            }

            //Session.SendPacket(new VoucherRedeemOkComposer());
        }
    }
}
