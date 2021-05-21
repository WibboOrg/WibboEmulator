namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class YoutubeTvComposer : ServerPacket
    {
        public YoutubeTvComposer(int ItemId, string VideoId)
            : base(3)
        {
            this.WriteInteger(ItemId);
            this.WriteString(VideoId);
        }
    }
}
