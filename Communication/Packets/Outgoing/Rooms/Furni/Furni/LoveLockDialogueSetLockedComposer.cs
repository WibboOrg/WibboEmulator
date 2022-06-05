namespace Wibbo.Communication.Packets.Outgoing.Rooms.Furni.Furni
{
    internal class LoveLockDialogueSetLockedComposer : ServerPacket
    {
        public LoveLockDialogueSetLockedComposer(int ItemId)
            : base(ServerPacketHeader.LOVELOCK_FURNI_FRIEND_COMFIRMED)
        {
            this.WriteInteger(ItemId);
        }
    }
}
