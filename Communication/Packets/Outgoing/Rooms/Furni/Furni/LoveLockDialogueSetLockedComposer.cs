namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Furni;

internal sealed class LoveLockDialogueSetLockedComposer : ServerPacket
{
    public LoveLockDialogueSetLockedComposer(int itemId)
        : base(ServerPacketHeader.LOVELOCK_FURNI_FRIEND_COMFIRMED) => this.WriteInteger(itemId);
}
