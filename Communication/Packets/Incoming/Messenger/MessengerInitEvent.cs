using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class MessengerInitEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetMessenger().OnStatusChanged();

            Session.SendPacket(Session.GetHabbo().GetMessenger().SerializeCategories());
            Session.SendPacket(Session.GetHabbo().GetMessenger().SerializeFriends());
            Session.GetHabbo().GetMessenger().ProcessOfflineMessages();
        }
    }
}
