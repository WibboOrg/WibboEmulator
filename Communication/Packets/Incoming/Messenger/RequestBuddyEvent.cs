namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal sealed class RequestBuddyEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var userName = packet.PopString(16);

        if (Session.User.Messenger == null || !Session.User.Messenger.RequestBuddy(userName))
        {
            return;
        }

        QuestManager.ProgressUserQuest(Session, QuestType.SocialFriend, 0);
    }
}
