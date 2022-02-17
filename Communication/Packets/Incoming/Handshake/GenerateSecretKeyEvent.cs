using Buttefly.Communication.Encryption;
using Buttefly.Communication.Encryption.Crypto.Prng;
using Buttefly.Utilities;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GenerateSecretKeyEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string CipherPublickey = Packet.PopString();

            BigInteger SharedKey = EncryptionV2.CalculateDiffieHellmanSharedKey(CipherPublickey);
            if (SharedKey != 0)
            {
                Session.RC4Client = new ARC4(SharedKey.getBytes());
                Session.SendPacket(new SecretKeyComposer(EncryptionV2.GetRsaDiffieHellmanPublicKey()));
            }
        }
    }
}