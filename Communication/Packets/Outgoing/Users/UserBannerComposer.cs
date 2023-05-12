namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal sealed class UserBannerComposer : ServerPacket
{
    public UserBannerComposer(int userId, int bannerId)
        : base(ServerPacketHeader.USER_BANNER)
    {
        this.WriteInteger(userId);
        this.WriteInteger(bannerId);
    }
}
