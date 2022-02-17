using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class DeclineBuddyEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            bool DeleteAllFriend = Packet.PopBoolean();
            int RequestCount = Packet.PopInt();

            if (!DeleteAllFriend && RequestCount == 1)
            {
                Session.GetHabbo().GetMessenger().HandleRequest(Packet.PopInt());
            }
            else
            {
                Session.GetHabbo().GetMessenger().HandleAllRequests();
            }
        }
    }
}