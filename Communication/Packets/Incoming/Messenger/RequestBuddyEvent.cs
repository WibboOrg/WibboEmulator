using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RequestBuddyEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string UserName = Packet.PopString();

            if (Session.GetUser().GetMessenger() == null || !Session.GetUser().GetMessenger().RequestBuddy(UserName))
            {
                return;
            }

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_FRIEND, 0);
        }
    }
}
