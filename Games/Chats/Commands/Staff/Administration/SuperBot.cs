namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal sealed class SuperBot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {

        if (parameters.Length > 1)
        {
            int count;
            if (parameters.Length == 2)
            {
                _ = int.TryParse(parameters[1], out count);
                for (var i = 0; i < count; i++)
                {
                    if (!room.IsRoleplay)
                    {
                        var superBot = room.RoomUserManager.DeploySuperBot(new RoomBot(-i, session.User.Id, room.Id, BotAIType.SuperBot, false, session.User.Username, "SuperBot", session.User.Gender, session.User.Look, userRoom.X, userRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.BotData.FollowUser = userRoom.VirtualId;
                    }
                    else
                    {
                        _ = room.RoomUserManager.DeploySuperBot(new RoomBot(-i, session.User.Id, room.Id, BotAIType.SuperBot, false, session.User.Username, "SuperBot", session.User.Gender, session.User.Look, userRoom.X, userRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    }
                }
            }
            else if (parameters.Length > 2)
            {
                var getUserRoom = session.User.Room.RoomUserManager.GetRoomUserByName(parameters[1]);
                if (getUserRoom == null)
                {
                    return;
                }

                if (session.Language != getUserRoom.Client.Language)
                {
                    session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.authorized.langue.user", session.Language), getUserRoom.Client.Language));
                    return;
                }

                _ = int.TryParse(parameters[2], out count);
                for (var i = 0; i < count; i++)
                {
                    var superBot = room.RoomUserManager.DeploySuperBot(new RoomBot(-i, getUserRoom.Client.User.Id, room.Id, BotAIType.SuperBot, false, getUserRoom.Client.User.Username, "SuperBot", getUserRoom.Client.User.Gender, getUserRoom.Client.User.Look, getUserRoom.X, getUserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    superBot.BotData.FollowUser = getUserRoom.VirtualId;
                }
            }
        }
        else
        {
            var users = GameClientManager.Clients.ToList();

            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                if (user.User == null)
                {
                    continue;
                }

                var randomWalkableSquare = room.GameMap.GetRandomWalkableSquare(userRoom.X, userRoom.Y);

                var superBot = room.RoomUserManager.DeploySuperBot(new RoomBot(0, session.User.Id, room.Id, BotAIType.SuperBot, false, user.User.Username, user.User.Motto, user.User.Gender, user.User.Look, userRoom.X, userRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                superBot.MoveTo(randomWalkableSquare, true);
            }
        }
    }
}
