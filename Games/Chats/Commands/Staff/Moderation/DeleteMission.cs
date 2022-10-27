namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DeleteMission : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];
        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(username);
        if (targetUser == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (session.User.Rank <= targetUser.User.Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.notpermitted", session.Langue));
        }
        else
        {
            targetUser.User.Motto = WibboEnvironment.GetLanguageManager().TryGetValue("user.unacceptable_motto", targetUser.Langue);
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateMotto(dbClient, targetUser.User.Id, targetUser.User.Motto);
            }

            var currentRoom2 = targetUser.User.CurrentRoom;
            if (currentRoom2 == null)
            {
                return;
            }

            var roomUserByUserId = currentRoom2.RoomUserManager.GetRoomUserByUserId(targetUser.User.Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            currentRoom2.SendPacket(new UserChangeComposer(roomUserByUserId, false));
        }
    }
}
