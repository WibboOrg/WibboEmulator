namespace WibboEmulator.Communication.Packets.Outgoing.Televisions;

internal sealed class YoutubeTvComposer : ServerPacket
{
    public YoutubeTvComposer(int itemId, string videoId)
        : base(ServerPacketHeader.YOUTUBE_TV)
    {
        this.WriteInteger(itemId);
        this.WriteString(videoId);
    }
}
