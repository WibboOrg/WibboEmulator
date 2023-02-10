namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;

internal sealed class InstantMessageErrorComposer : ServerPacket
{
    public InstantMessageErrorComposer(int error, int target)
        : base(ServerPacketHeader.MESSENGER_INSTANCE_MESSAGE_ERROR)
    {
        this.WriteInteger(error);
        this.WriteInteger(target);
        this.WriteString("");
    }
}
