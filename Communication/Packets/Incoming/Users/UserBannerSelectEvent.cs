namespace WibboEmulator.Communication.Packets.Incoming.Users;

using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;

internal sealed class UserBannerSelectEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var bannerId = packet.PopInt();

        if (Session == null || Session.User == null || Session.User.BannerComponent == null)
        {
            return;
        }

        if (!BannerManager.TryGetBannerById(bannerId, out var banner) && bannerId != -1)
        {
            return;
        }

        if (!Session.User.BannerComponent.BannerList.Contains(banner) && bannerId != -1)
        {
            return;
        }

        Session.User.BannerSelected = bannerId != -1 ? banner : null;

        Session.SendPacket(new UserBannerComposer(Session.User.Id, Session.User.BannerSelected));
    }
}
