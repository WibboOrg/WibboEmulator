namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal sealed class RequestBuddyEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var userName = packet.PopString(16);

        if (session.User.Messenger == null || !session.User.Messenger.RequestBuddy(userName))
        {
            return;
        }

        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialFriend, 0);
    }
}
