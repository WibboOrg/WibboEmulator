using Butterfly.Game.Clients;
using Butterfly.Game.Users.Messenger;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class AcceptBuddyEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int Count = Packet.PopInt();
            for (int index = 0; index < Count; ++index)
            {
                int num2 = Packet.PopInt();
                MessengerRequest request = Session.GetHabbo().GetMessenger().GetRequest(num2);
                if (request != null)
                {
                    if (request.To != Session.GetHabbo().Id)
                    {
                        break;
                    }

                    if (!Session.GetHabbo().GetMessenger().FriendshipExists(request.To))
                    {
                        Session.GetHabbo().GetMessenger().CreateFriendship(request.From);
                    }

                    Session.GetHabbo().GetMessenger().HandleRequest(num2);
                }
            }
        }
    }
}