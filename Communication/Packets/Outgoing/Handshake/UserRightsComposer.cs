namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;

internal class UserRightsComposer : ServerPacket
{
    public UserRightsComposer(int rank)
        : base(ServerPacketHeader.USER_PERMISSIONS)
    {
        this.WriteInteger(2);//Club level
        this.WriteInteger(rank);
        this.WriteBoolean(false);//Is an ambassador
    }
}
