namespace WibboEmulator.Communication.Packets.Outgoing.Sound.SoundCustom
{
    internal class StopSoundComposer : ServerPacket
    {
        public StopSoundComposer(string Name)
            : base(ServerPacketHeader.STOP_SOUND)
        {
            this.WriteString(Name);
        }
    }
}