using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UniqueIDEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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
