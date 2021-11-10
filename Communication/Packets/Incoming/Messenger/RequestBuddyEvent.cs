using Butterfly.Game.GameClients;
using Butterfly.Game.Quests;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RequestBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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
