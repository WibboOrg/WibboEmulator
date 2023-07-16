namespace WibboEmulator.Communication.Packets.Outgoing.Users;

using WibboEmulator.Games.Banners;

internal sealed class UserBannerComposer : ServerPacket
{
    public UserBannerComposer(int userId, Banner banner)
        : base(ServerPacketHeader.USER_BANNER)
    {
        this.WriteInteger(userId);
        this.WriteInteger(banner?.Id ?? -1);
        this.WriteBoolean(banner?.HaveLayer ?? false);
    }
}
