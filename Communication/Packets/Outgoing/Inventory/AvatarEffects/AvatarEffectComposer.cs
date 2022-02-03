namespace Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectComposer : ServerPacket
    {
        public AvatarEffectComposer(int virtualId, int effectID)
            : base(ServerPacketHeader.UNIT_EFFECT)
        {
            this.WriteInteger(virtualId);
            this.WriteInteger(effectID);
            this.WriteInteger(0);
        }
    }
}
