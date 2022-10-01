using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Quests;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RequestBuddyEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
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
