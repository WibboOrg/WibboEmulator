namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class DeleteMission : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];
        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(username);
        if (TargetUser == null)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", session.Langue));
        }
        else if (session.GetUser().Rank <= TargetUser.GetUser().Rank)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.notpermitted", session.Langue));
        }
        else
        {
            TargetUser.GetUser().Motto = WibboEnvironment.GetLanguageManager().TryGetValue("user.unacceptable_motto", TargetUser.Langue);
            using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateMotto(dbClient, TargetUser.GetUser().Id, TargetUser.GetUser().Motto);
            }

            var currentRoom2 = TargetUser.GetUser().CurrentRoom;
            if (currentRoom2 == null)
            {
                return;
            }

            var roomUserByUserId = currentRoom2.GetRoomUserManager().GetRoomUserByUserId(TargetUser.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            currentRoom2.SendPacket(new UserChangeComposer(roomUserByUserId, false));
        }
    }
}
