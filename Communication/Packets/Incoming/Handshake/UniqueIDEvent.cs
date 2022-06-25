using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
