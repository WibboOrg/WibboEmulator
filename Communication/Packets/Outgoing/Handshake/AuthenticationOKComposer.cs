namespace Wibbo.Communication.Packets.Outgoing.Handshake
{
    internal class AuthenticationOKComposer : ServerPacket
    {
        public AuthenticationOKComposer()
            : base(ServerPacketHeader.AUTHENTICATED)
        {

        }
    }
}
