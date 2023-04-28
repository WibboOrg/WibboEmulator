namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal sealed class UserRightsComposer : ServerPacket
{
    public UserRightsComposer(int rank, bool isPremium)
        : base(ServerPacketHeader.USER_PERMISSIONS)
    {
        this.WriteInteger(isPremium ? 2 : 1);//Club level
        this.WriteInteger(rank);
        this.WriteBoolean(false);//Is an ambassador
    }
}
