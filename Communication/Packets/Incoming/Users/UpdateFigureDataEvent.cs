namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class UpdateFigureDataEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.GetUser() == null)
        {
            return;
        }

        var Gender = packet.PopString().ToUpper();
        var Look = packet.PopString();
        if (Gender is not "M" and not "F")
        {
            return;
        }

        Look = WibboEnvironment.GetFigureManager().ProcessFigure(Look, Gender, true);

        WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.PROFILE_CHANGE_LOOK, 0);

        session.GetUser().Look = Look;
        session.GetUser().Gender = Gender.ToLower();

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateLookAndGender(dbClient, session.GetUser().Id, Look, Gender);
        }

        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_AvatarLooks", 1);

        if (!session.GetUser().InRoom)
        {
            return;
        }

        var currentRoom = session.GetUser().CurrentRoom;

        if (currentRoom == null)
        {
            return;
        }

        var roomUserByUserId = currentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
        {
            return;
        }

        session.SendPacket(new FigureUpdateComposer(session.GetUser().Look, session.GetUser().Gender));
        session.SendPacket(new UserObjectComposer(session.GetUser()));
        session.SendPacket(new UserChangeComposer(roomUserByUserId, true));

        currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
    }
}