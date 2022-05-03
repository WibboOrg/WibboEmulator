namespace Butterfly.Communication.Packets.Outgoing.Televisions
{
    internal class YoutubeTvComposer : ServerPacket
    {
        public YoutubeTvComposer(int ItemId, string VideoId)
            : base(ServerPacketHeader.YOUTUBE_TV)
        {
            this.WriteInteger(ItemId);
            this.WriteString(VideoId);
        }
    }
}
