using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class DeclineBuddyEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetUser().GetMessenger() == null)
            {
                return;
            }

            bool DeleteAllFriend = Packet.PopBoolean();
            int RequestCount = Packet.PopInt();

            if (!DeleteAllFriend && RequestCount == 1)
            {
                Session.GetUser().GetMessenger().HandleRequest(Packet.PopInt());
            }
            else
            {
                Session.GetUser().GetMessenger().HandleAllRequests();
            }
        }
    }
}