namespace WibboEmulator.Communication.Packets.Incoming.Users;

using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users;

internal sealed class UserBannerEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userId = packet.PopInt();
        var all = packet.PopBoolean();

        var user = UserManager.GetUserById(userId);
        if (user == null)
        {
            return;
        }

        if (all && user.BannerComponent != null)
        {
            session.SendPacket(new UserBannerListComposer(user.BannerComponent.BannerList));
        }

        session.SendPacket(new UserBannerComposer(user.Id, user.BannerSelected));
    }
}
