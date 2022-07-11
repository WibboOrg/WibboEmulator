using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Game.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetFurnitureAliasesMessageEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new FurnitureAliasesComposer());
        }
    }
}