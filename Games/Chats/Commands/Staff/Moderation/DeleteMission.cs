namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class DeleteMission : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];
        var TargetUser = GameClientManager.GetClientByUsername(username);
        if (TargetUser == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
        }
        else if (Session.User.Rank <= TargetUser.User.Rank)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("user.notpermitted", Session.Language));
        }
        else
        {
            TargetUser.User.Motto = LanguageManager.TryGetValue("user.unacceptable_motto", TargetUser.Language);
            using (var dbClient = DatabaseManager.Connection)
            {
                UserDao.UpdateMotto(dbClient, TargetUser.User.Id, TargetUser.User.Motto);
            }

            var currentRoom2 = TargetUser.User.Room;
            if (currentRoom2 == null)
            {
                return;
            }

            var roomUserByUserId = currentRoom2.RoomUserManager.GetRoomUserByUserId(TargetUser.User.Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            currentRoom2.SendPacket(new UserChangeComposer(roomUserByUserId, false));
        }
    }
}
