namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;

internal class SuperBot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {

        if (parameters.Length > 1)
        {
            int count;
            if (parameters.Length == 2)
            {
                _ = int.TryParse(parameters[1], out count);
                for (var i = 0; i < count; i++)
                {
                    if (!Room.IsRoleplay)
                    {
                        var superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, session.GetUser().Id, Room.Id, BotAIType.SuperBot, false, session.GetUser().Username, "SuperBot", session.GetUser().Gender, session.GetUser().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.BotData.FollowUser = UserRoom.VirtualId;
                    }
                    else
                    {
                        _ = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, session.GetUser().Id, Room.Id, BotAIType.SuperBot, false, session.GetUser().Username, "SuperBot", session.GetUser().Gender, session.GetUser().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    }
                }
            }
            else if (parameters.Length > 2)
            {
                var GetUserRoom = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(parameters[1]);
                if (GetUserRoom == null)
                {
                    return;
                }

                if (session.Langue != GetUserRoom.GetClient().Langue)
                {
                    session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", GetUserRoom.GetClient().Langue), session.Langue));
                    return;
                }

                _ = int.TryParse(parameters[2], out count);
                for (var i = 0; i < count; i++)
                {
                    var superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, GetUserRoom.GetClient().GetUser().Id, Room.Id, BotAIType.SuperBot, false, GetUserRoom.GetClient().GetUser().Username, "SuperBot", GetUserRoom.GetClient().GetUser().Gender, GetUserRoom.GetClient().GetUser().Look, GetUserRoom.X, GetUserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    superBot.BotData.FollowUser = GetUserRoom.VirtualId;
                }
            }
        }
        else
        {
            var Users = WibboEnvironment.GetGame().GetGameClientManager().GetClients;

            if (Users == null)
            {
                return;
            }

            foreach (var GameClient in Users)
            {
                if (GameClient.GetUser() == null)
                {
                    continue;
                }

                var randomWalkableSquare = Room.GetGameMap().GetRandomWalkableSquare(UserRoom.X, UserRoom.Y);

                var superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(0, session.GetUser().Id, Room.Id, BotAIType.SuperBot, false, GameClient.GetUser().Username, GameClient.GetUser().Motto, GameClient.GetUser().Gender, GameClient.GetUser().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                superBot.MoveTo(randomWalkableSquare, true);
            }
        }
    }
}
