namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni
{
    internal class LoveLockDialogueCloseMessageComposer : ServerPacket
    {
        public LoveLockDialogueCloseMessageComposer(int ItemId)
            : base(ServerPacketHeader.LOVELOCK_FURNI_FINISHED)
        {
            this.WriteInteger(ItemId);
        }
    }
}
