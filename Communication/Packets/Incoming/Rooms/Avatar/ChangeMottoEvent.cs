namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal sealed class ChangeMottoEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var newMotto = packet.PopString(38);
        if (newMotto == Session.User.Motto)
        {
            return;
        }

        if (!Session.User.HasPermission("word_filter_override"))
        {
            newMotto = WordFilterManager.CheckMessage(newMotto);
        }

        if (Session.User.IgnoreAll)
        {
            return;
        }

        if (Session.User.CheckChatMessage(newMotto, "<MOTTO>"))
        {
            return;
        }

        Session.User.Motto = newMotto;

        using (var dbClient = DatabaseManager.Connection)
        {
            UserDao.UpdateMotto(dbClient, Session.User.Id, newMotto);
        }

        QuestManager.ProgressUserQuest(Session, QuestType.ProfileChangeMotto, 0);

        if (Session.User.InRoom)
        {
            var currentRoom = Session.User.Room;
            if (currentRoom == null)
            {
                return;
            }

            var roomUserByUserId = currentRoom.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
            {
                return;
            }

            currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
        }

        _ = AchievementManager.ProgressAchievement(Session, "ACH_Motto", 1);
    }
}
