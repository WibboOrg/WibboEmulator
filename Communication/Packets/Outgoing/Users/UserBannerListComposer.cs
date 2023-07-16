namespace WibboEmulator.Communication.Packets.Outgoing.Users;

using WibboEmulator.Games.Banners;

internal sealed class UserBannerListComposer : ServerPacket
{
    public UserBannerListComposer(List<Banner> bannerList)
        : base(ServerPacketHeader.USER_BANNER_LIST)
    {
        this.WriteInteger(bannerList.Count);
        foreach (var banner in bannerList)
        {
            this.WriteInteger(banner.Id);
            this.WriteBoolean(banner.HaveLayer);
        }
    }
}
