namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;

internal sealed class LoveLockDialogueComposer : ServerPacket
{
    public LoveLockDialogueComposer(int itemId)
        : base(ServerPacketHeader.LOVELOCK_FURNI_START)
    {
        this.WriteInteger(itemId);
        this.WriteBoolean(true);
    }
}
