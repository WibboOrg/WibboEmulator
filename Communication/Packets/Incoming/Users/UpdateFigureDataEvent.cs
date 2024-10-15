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

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User == null)
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

        QuestManager.ProgressUserQuest(Session, QuestType.ProfileChangeLook, 0);

        Session.User.Look = look;
        Session.User.Gender = gender;

        using (var dbClient = DatabaseManager.Connection)
        {
            UserDao.UpdateLookAndGender(dbClient, Session.User.Id, look, gender);
        }

        _ = AchievementManager.ProgressAchievement(Session, "ACH_AvatarLooks", 1);

        if (!Session.User.InRoom)
        {
            return;
        }

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

        Session.SendPacket(new FigureUpdateComposer(Session.User.Look, Session.User.Gender));
        Session.SendPacket(new UserObjectComposer(Session.User));
        Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));

        currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));
    }
}
