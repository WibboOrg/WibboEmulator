using WibboEmulator.Games.Clients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class OpenFlatConnectionEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            int RoomId = Packet.PopInt();
            string Password = Packet.PopString();

            Session.GetUser().PrepareRoom(RoomId, Password);
        }
    }
}