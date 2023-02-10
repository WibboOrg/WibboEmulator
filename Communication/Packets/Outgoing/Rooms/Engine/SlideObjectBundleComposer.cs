namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal sealed class SlideObjectBundleComposer : ServerPacket
{
    public SlideObjectBundleComposer(int x, int y, double z, int nextX, int nextY, double nextHeight, int id, int rollerId = 0, bool isItem = true)
        : base(ServerPacketHeader.ROOM_ROLLING)
    {
        this.WriteInteger(x);
        this.WriteInteger(y);
        this.WriteInteger(nextX);
        this.WriteInteger(nextY);

        this.WriteInteger(isItem ? 1 : 0);
        if (isItem)
        {
            this.WriteInteger(id);
            this.WriteString(z.ToString());
            this.WriteString(nextHeight.ToString());
        }

        this.WriteInteger(rollerId);

        if (isItem)
        {
            return;
        }

        this.WriteInteger(2);
        this.WriteInteger(id);
        this.WriteString(z.ToString());
        this.WriteString(nextHeight.ToString());
    }
}
