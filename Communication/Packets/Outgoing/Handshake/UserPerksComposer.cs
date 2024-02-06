namespace WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Games.Users;

internal sealed class UserPerksComposer : ServerPacket
{
    public UserPerksComposer(User user)
        : base(ServerPacketHeader.USER_PERKS)
    {
        this.WriteInteger(1); // Count

        this.WriteString("USE_GUIDE_TOOL");
        this.WriteString("");
        this.WriteBoolean(user.HasPermission("helptool"));
    }
}
