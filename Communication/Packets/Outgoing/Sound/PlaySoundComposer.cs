namespace WibboEmulator.Communication.Packets.Outgoing.Sound;

internal sealed class PlaySoundComposer : ServerPacket
{
    public PlaySoundComposer(string name, int type, bool loop = false)
        : base(ServerPacketHeader.PLAY_SOUND)
    {
        this.WriteString(name);
        this.WriteInteger(type);
        this.WriteBoolean(loop);
    }
}
