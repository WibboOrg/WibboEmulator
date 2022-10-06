namespace WibboEmulator.Communication.Packets.Outgoing.Sound;

internal class PlaySoundComposer : ServerPacket
{
    public PlaySoundComposer(string Name, int Type, bool Loop = false)
        : base(ServerPacketHeader.PLAY_SOUND)
    {
        this.WriteString(Name);
        this.WriteInteger(Type);
        this.WriteBoolean(Loop);
    }
}
