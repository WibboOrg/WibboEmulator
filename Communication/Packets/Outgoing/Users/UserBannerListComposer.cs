namespace WibboEmulator.Communication.Packets.Outgoing.Users;

internal sealed class UserBannerListComposer : ServerPacket
{
    public UserBannerListComposer(List<int> bannerList)
        : base(ServerPacketHeader.USER_BANNER_LIST)
    {
        this.WriteInteger(bannerList.Count);
        foreach (var id in bannerList)
        {
            this.WriteInteger(id);
        }
    }
}
