using Butterfly.Game.Clients;
using Butterfly.Game.Quests;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RequestBuddyEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string UserName = Packet.PopString();

            if (Session.GetHabbo().GetMessenger() == null || !Session.GetHabbo().GetMessenger().RequestBuddy(UserName))
            {
                return;
            }

            ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_FRIEND, 0);
        }
    }
}
