namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal class SuperBot : IChatCommand
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
                        var superBot = room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, session.GetUser().Id, room.Id, BotAIType.SuperBot, false, session.GetUser().Username, "SuperBot", session.GetUser().Gender, session.GetUser().Look, userRoom.X, userRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.BotData.FollowUser = userRoom.VirtualId;
                    }
                    else
                    {
                        _ = room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, session.GetUser().Id, room.Id, BotAIType.SuperBot, false, session.GetUser().Username, "SuperBot", session.GetUser().Gender, session.GetUser().Look, userRoom.X, userRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    }
                }
            }
            else if (parameters.Length > 2)
            {
                var getUserRoom = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(parameters[1]);
                if (getUserRoom == null)
                {
                    return;
                }

                if (session.Langue != getUserRoom.GetClient().Langue)
                {
                    session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue.user", session.Langue), getUserRoom.GetClient().Langue));
                    return;
                }

                _ = int.TryParse(parameters[2], out count);
                for (var i = 0; i < count; i++)
                {
                    var superBot = room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, getUserRoom.GetClient().GetUser().Id, room.Id, BotAIType.SuperBot, false, getUserRoom.GetClient().GetUser().Username, "SuperBot", getUserRoom.GetClient().GetUser().Gender, getUserRoom.GetClient().GetUser().Look, getUserRoom.X, getUserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    superBot.BotData.FollowUser = getUserRoom.VirtualId;
                }
            }
        }
        else
        {
            var users = WibboEnvironment.GetGame().GetGameClientManager().GetClients;

            if (users == null)
            {
                return;
            }

            foreach (var user in users)
            {
                if (user.GetUser() == null)
                {
                    continue;
                }

                var randomWalkableSquare = room.GetGameMap().GetRandomWalkableSquare(userRoom.X, userRoom.Y);

                var superBot = room.GetRoomUserManager().DeploySuperBot(new RoomBot(0, session.GetUser().Id, room.Id, BotAIType.SuperBot, false, user.GetUser().Username, user.GetUser().Motto, user.GetUser().Gender, user.GetUser().Look, userRoom.X, userRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                superBot.MoveTo(randomWalkableSquare, true);
            }
        }
    }
}
