namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni
{
    internal class LoveLockDialogueSetLockedMessageComposer : ServerPacket
    {
        public LoveLockDialogueSetLockedMessageComposer(int ItemId)
            : base(ServerPacketHeader.LOVELOCK_FURNI_FRIEND_COMFIRMED)
        {
            this.WriteInteger(ItemId);
        }
    }
}
