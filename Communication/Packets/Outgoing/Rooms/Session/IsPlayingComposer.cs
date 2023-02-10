namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;

internal sealed class IsPlayingComposer : ServerPacket
{
    public IsPlayingComposer(bool isPlaying)
        : base(ServerPacketHeader.PLAYING_GAME) => this.WriteBoolean(isPlaying);
}
