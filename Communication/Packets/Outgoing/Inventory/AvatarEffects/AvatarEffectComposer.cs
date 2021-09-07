namespace Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectComposer : ServerPacket
    {
        public AvatarEffectComposer(int playerID, int effectID)
            : base(ServerPacketHeader.UNIT_EFFECT)
        {
            this.WriteInteger(playerID);
            this.WriteInteger(effectID);
            this.WriteInteger(0);
        }
    }
}