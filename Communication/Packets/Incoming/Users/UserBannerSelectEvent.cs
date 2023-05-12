namespace WibboEmulator.Communication.Packets.Incoming.Users;

using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class UserBannerSelectEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var bannerId = packet.PopInt();

        if (session == null || session.User == null || session.User.Banner == null)
        {
            return;
        }

        if (!session.User.Banner.BannerList.Contains(bannerId))
        {
            return;
        }

        session.User.BannerId = bannerId;

        session.SendPacket(new UserBannerComposer(session.User.Id, session.User.BannerId));
    }
}
