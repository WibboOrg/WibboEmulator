namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;

internal class LoveLockDialogueCloseComposer : ServerPacket
{
    public LoveLockDialogueCloseComposer(int itemId)
        : base(ServerPacketHeader.LOVELOCK_FURNI_FINISHED) => this.WriteInteger(itemId);
}
