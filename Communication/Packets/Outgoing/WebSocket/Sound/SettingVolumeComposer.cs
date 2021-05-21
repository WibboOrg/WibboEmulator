namespace Butterfly.Communication.Packets.Outgoing.WebSocket
{
    internal class SettingVolumeComposer : ServerPacket
    {
        public SettingVolumeComposer(int Volume1, int Volume2, int Volume3)
            : base(20)
        {
            this.WriteInteger(Volume1);
            this.WriteInteger(Volume2);
            this.WriteInteger(Volume3);
        }
    }
}
