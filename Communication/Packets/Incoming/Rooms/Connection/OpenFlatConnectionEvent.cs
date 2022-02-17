using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class OpenFlatConnectionEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();

            Session.GetHabbo().PrepareRoom(RoomId, Password);
        }
    }
}