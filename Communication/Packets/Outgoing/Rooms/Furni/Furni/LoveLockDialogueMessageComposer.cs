namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni
{
    internal class LoveLockDialogueMessageComposer : ServerPacket
    {
        public LoveLockDialogueMessageComposer(int ItemId)
            : base(ServerPacketHeader.LOVELOCK_FURNI_START)
        {
            this.WriteInteger(ItemId);
            this.WriteBoolean(true);
        }
    }
}
