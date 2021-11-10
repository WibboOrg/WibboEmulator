using Butterfly.Game.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class MessengerInitEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetMessenger().OnStatusChanged();

            Session.SendPacket(Session.GetHabbo().GetMessenger().SerializeCategories());
            Session.SendPacket(Session.GetHabbo().GetMessenger().SerializeFriends());
            Session.GetHabbo().GetMessenger().ProcessOfflineMessages();
        }
    }
}
