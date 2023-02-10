namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal sealed class OpenHelpToolComposer : ServerPacket
{
    public OpenHelpToolComposer(int type)
        : base(ServerPacketHeader.CFH_PENDING_CALLS) => this.WriteInteger(type);
}
