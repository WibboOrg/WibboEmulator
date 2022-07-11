namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectsComposer : ServerPacket
    {
        public AvatarEffectsComposer(List<int> Enable)
            : base(ServerPacketHeader.USER_EFFECTS)
        {
            this.WriteInteger(Enable.Count);

            foreach (int EffectId in Enable)
            {
                this.WriteInteger(EffectId);//Effect Id
                this.WriteInteger(1);//Type, 0 = Hand, 1 = Full
                this.WriteInteger(0);
                this.WriteInteger(1);
                this.WriteInteger(-1);
                this.WriteBoolean(true);//Permanent
            }
        }
    }
}
