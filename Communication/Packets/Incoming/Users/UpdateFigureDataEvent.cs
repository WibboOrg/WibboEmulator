namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Handshake;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.FigureData;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal sealed class UpdateFigureDataEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User == null)
        {
            return;
        }

        var gender = packet.PopString(1).ToUpper();
        var look = packet.PopString();

        if (gender is not "M" and not "F")
        {
            return;
        }

        look = FigureDataManager.ProcessFigure(look, gender, true);

        QuestManager.ProgressUserQuest(session, QuestType.ProfileChangeLook, 0);

        session.User.Look = look;
        session.User.Gender = gender;

        using (var dbClient = DatabaseManager.Connection)
        {
            UserDao.UpdateLookAndGender(dbClient, session.User.Id, look, gender);
        }

        _ = AchievementManager.ProgressAchievement(session, "ACH_AvatarLooks", 1);

        if (!session.User.InRoom)
        {
            return;
        }

        var currentRoom = session.User.Room;

        if (currentRoom == null)
        {
            return;
        }

        var roomUserByUserId = currentRoom.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        if (roomUserByUserId.IsTransf || roomUserByUserId.IsSpectator)
        {
            return;
        }

        session.SendPacket(new FigureUpdateComposer(session.User.Look, session.User.Gender));
        session.SendPacket(new UserObjectComposer(session.User));
        session.SendPacket(new UserChangeComposer(roomUserByUserId, true));

        currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
    }
}
