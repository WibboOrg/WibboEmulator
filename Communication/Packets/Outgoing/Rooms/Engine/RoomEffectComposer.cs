namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;

internal sealed class RoomEffectComposer : ServerPacket
{
    public RoomEffectComposer(int type)
        : base(ServerPacketHeader.ROOM_EFFECT) => this.WriteInteger(type);
}
