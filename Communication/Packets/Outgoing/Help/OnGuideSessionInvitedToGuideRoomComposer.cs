namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal sealed class OnGuideSessionInvitedToGuideRoomComposer : ServerPacket
{
    public OnGuideSessionInvitedToGuideRoomComposer(int roomId, string name)
        : base(ServerPacketHeader.GUIDE_SESSION_INVITED_TO_GUIDE_ROOM)
    {
        this.WriteInteger(roomId);
        this.WriteString(name);
    }
}
