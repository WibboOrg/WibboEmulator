using Buttefly.Communication.Encryption;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class InitCryptoEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new InitCryptoComposer(EncryptionV2.GetRsaDiffieHellmanPrimeKey(), EncryptionV2.GetRsaDiffieHellmanGeneratorKey()));
        }
    }
}