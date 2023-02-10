namespace WibboEmulator.Communication.Packets.Outgoing.Sound;

internal sealed class StopSoundComposer : ServerPacket
{
    public StopSoundComposer(string name)
        : base(ServerPacketHeader.STOP_SOUND) => this.WriteString(name);
}
