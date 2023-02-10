namespace WibboEmulator.Communication.Packets.Outgoing.Camera;

internal sealed class ThumbnailStatusComposer : ServerPacket
{
    public ThumbnailStatusComposer(bool ok, bool renderLimitHit)
        : base(ServerPacketHeader.THUMBNAIL_STATUS)
    {
        this.WriteBoolean(ok);
        this.WriteBoolean(renderLimitHit);
    }

    public ThumbnailStatusComposer()
        : base(ServerPacketHeader.THUMBNAIL_STATUS)
    {
    }
}