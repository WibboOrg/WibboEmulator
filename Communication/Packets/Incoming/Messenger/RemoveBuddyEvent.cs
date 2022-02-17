using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveBuddyEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            int count = Packet.PopInt();

            if (count > 200) count = 200;

            int friendId;
            for (int index = 0; index < count; index++)
            {
                friendId = Packet.PopInt();
                Session.GetHabbo().GetMessenger().DestroyFriendship(friendId);
            }
        }
    }
}
