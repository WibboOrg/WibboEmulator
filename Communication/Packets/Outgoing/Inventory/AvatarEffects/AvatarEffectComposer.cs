namespace Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectComposer : ServerPacket
    {
        public AvatarEffectComposer(int virtualId, int effectID, int delay = 0)
            : base(ServerPacketHeader.UNIT_EFFECT)
        {
            this.WriteInteger(virtualId);
            this.WriteInteger(effectID);
            this.WriteInteger(delay);
        }
    }
}
