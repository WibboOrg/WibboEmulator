namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;

internal sealed class RoomInviteComposer : ServerPacket
{
    public RoomInviteComposer(int senderId, string textMessage)
        : base(ServerPacketHeader.MESSENGER_ROOM_INVITE)
    {
        this.WriteInteger(senderId);
        this.WriteString(textMessage);
    }
}
