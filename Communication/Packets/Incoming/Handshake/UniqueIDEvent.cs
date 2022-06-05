using Wibbo.Communication.Packets.Outgoing.Handshake;
using Wibbo.Game.Clients;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class UniqueIDEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string CookieId = Packet.PopString();
            string McId = Packet.PopString();
            string Junk = Packet.PopString();

            string Head = (string.IsNullOrWhiteSpace(CookieId) || CookieId.Length != 13) ? IDGenerator.Instance.Next : CookieId;

            Session.MachineId = Head + McId + Junk;

            Session.SendPacket(new SetUniqueIdComposer(Head));
        }
    }
}
