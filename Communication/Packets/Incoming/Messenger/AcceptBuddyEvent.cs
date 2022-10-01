using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Users.Messenger;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class AcceptBuddyEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser().GetMessenger() == null)
            {
                return;
            }

            int Count = Packet.PopInt();
            for (int index = 0; index < Count; ++index)
            {
                int num2 = Packet.PopInt();
                MessengerRequest request = Session.GetUser().GetMessenger().GetRequest(num2);
                if (request != null)
                {
                    if (request.To != Session.GetUser().Id)
                    {
                        break;
                    }

                    if (!Session.GetUser().GetMessenger().FriendshipExists(request.To))
                    {
                        Session.GetUser().GetMessenger().CreateFriendship(request.From);
                    }

                    Session.GetUser().GetMessenger().HandleRequest(num2);
                }
            }
        }
    }
}