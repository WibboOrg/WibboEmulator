namespace WibboEmulator.Communication.Packets.Outgoing.GameCenter;

internal class IsPlayingComposer : ServerPacket
{
    public IsPlayingComposer(bool isPlaying)
        : base(ServerPacketHeader.PLAYING_GAME) => this.WriteBoolean(isPlaying);
}