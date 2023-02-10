namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;

internal sealed class AvatarEffectsComposer : ServerPacket
{
    public AvatarEffectsComposer(List<int> enable)
        : base(ServerPacketHeader.USER_EFFECTS)
    {
        this.WriteInteger(enable.Count);

        foreach (var effectId in enable)
        {
            this.WriteInteger(effectId);//Effect Id
            this.WriteInteger(1);//Type, 0 = Hand, 1 = Full
            this.WriteInteger(0);
            this.WriteInteger(1);
            this.WriteInteger(-1);
            this.WriteBoolean(true);//Permanent
        }
    }
}
