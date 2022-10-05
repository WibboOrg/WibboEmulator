namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;

internal class LoveLockDialogueCloseComposer : ServerPacket
{
    public LoveLockDialogueCloseComposer(int ItemId)
        : base(ServerPacketHeader.LOVELOCK_FURNI_FINISHED) => this.WriteInteger(ItemId);
}
