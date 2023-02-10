namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;

internal sealed class LoveLockDialogueCloseComposer : ServerPacket
{
    public LoveLockDialogueCloseComposer(int itemId)
        : base(ServerPacketHeader.LOVELOCK_FURNI_FINISHED) => this.WriteInteger(itemId);
}
