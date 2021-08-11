namespace Butterfly.Communication.Packets.Outgoing.Handshake
{
    internal class InitCryptoComposer : ServerPacket
    {
        public InitCryptoComposer(string Prime, string Generator)
            : base(ServerPacketHeader.InitCryptoMessageComposer)
        {
            this.WriteString(Prime);
            this.WriteString(Generator);
        }
    }
}
