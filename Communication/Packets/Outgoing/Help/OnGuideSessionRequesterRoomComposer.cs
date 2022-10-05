namespace WibboEmulator.Communication.Packets.Outgoing.Help;

internal class OnGuideSessionRequesterRoomComposer : ServerPacket
{
    public OnGuideSessionRequesterRoomComposer(int roomId)
        : base(ServerPacketHeader.GUIDE_SESSION_REQUESTER_ROOM) => this.WriteInteger(roomId);
}
