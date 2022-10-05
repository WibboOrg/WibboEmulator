namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;

internal class InstantMessageErrorComposer : ServerPacket
{
    public InstantMessageErrorComposer(int Error, int Target)
        : base(ServerPacketHeader.MESSENGER_INSTANCE_MESSAGE_ERROR)
    {
        this.WriteInteger(Error);
        this.WriteInteger(Target);
        this.WriteString("");
    }
}
