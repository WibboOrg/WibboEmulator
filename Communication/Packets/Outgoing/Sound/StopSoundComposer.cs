namespace WibboEmulator.Communication.Packets.Outgoing.Sound;

internal class StopSoundComposer : ServerPacket
{
    public StopSoundComposer(string name)
        : base(ServerPacketHeader.STOP_SOUND) => this.WriteString(name);
}
