namespace WibboEmulator.Communication.Packets.Outgoing.Misc;

internal sealed class NuxAlertComposer : ServerPacket
{
    public NuxAlertComposer(int type)
        : base(ServerPacketHeader.NUX_ALERT_COMPOSER) => this.WriteInteger(type);
}
