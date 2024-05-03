namespace WibboEmulator.Communication.Packets.Incoming.Users;

using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.GameClients;

internal sealed class UserBannerSelectEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var bannerId = packet.PopInt();

        if (session == null || session.User == null || session.User.BannerComponent == null)
        {
            return;
        }

        if (!BannerManager.TryGetBannerById(bannerId, out var banner))
        {
            return;
        }

        if (!session.User.BannerComponent.BannerList.Contains(banner) && bannerId != -1)
        {
            return;
        }

        session.User.BannerSelected = bannerId != -1 ? banner : null;

        session.SendPacket(new UserBannerComposer(session.User.Id, session.User.BannerSelected));
    }
}
