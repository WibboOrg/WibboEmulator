namespace WibboEmulator.Communication.Packets.Incoming.Users;

using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal sealed class UserBannerEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var all = packet.PopBoolean();

        var user = WibboEnvironment.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        if (user.Banner == null)
        {
            return;
        }

        if (all)
        {
            session.SendPacket(new UserBannerListComposer(user.Banner.BannerList));
        }

        session.SendPacket(new UserBannerComposer(user.Id, user.BannerId));
    }
}
