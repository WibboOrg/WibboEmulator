namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class MaxFloor : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        var Map = "";
        var TailleFloor = 50;
        if (session.GetUser().Rank > 1)
        {
            TailleFloor = 75;
        }

        for (var y = 0; y < (Room.GetGameMap().Model.MapSizeY > TailleFloor ? Room.GetGameMap().Model.MapSizeY : TailleFloor); y++)
        {
            var Line = "";
            for (var x = 0; x < (Room.GetGameMap().Model.MapSizeX > TailleFloor ? Room.GetGameMap().Model.MapSizeX : TailleFloor); x++)
            {
                if (x >= Room.GetGameMap().Model.MapSizeX || y >= Room.GetGameMap().Model.MapSizeY)
                {
                    Line += "0";
                }
                else
                {
                    if (Room.GetGameMap().Model.SqState[x, y] == SquareStateType.BLOCKED)
                    {
                        Line += "0";//x
                    }
                    else
                    {
                        Line += ParseInverse(Room.GetGameMap().Model.SqFloorHeight[x, y]);
                    }
                }
            }
            Map += Line + Convert.ToChar(13);
        }

        Map = Map.TrimEnd(Convert.ToChar(13));

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomModelCustomDao.Replace(dbClient, Room.Id, Room.GetGameMap().Model.DoorX, Room.GetGameMap().Model.DoorY, Room.GetGameMap().Model.DoorZ, Room.GetGameMap().Model.DoorOrientation, Map, Room.GetGameMap().Model.WallHeight);
            RoomDao.UpdateModel(dbClient, Room.Id);
        }

        var UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);


        foreach (var User in UsersToReturn)
        {
            if (User == null || User.GetClient() == null)
            {
                continue;
            }

            User.GetClient().SendPacket(new RoomForwardComposer(Room.Id));
        }
    }

    private static char ParseInverse(double input)
    {
        switch (input)
        {
            case 0:
                return '0';
            case 1:
                return '1';
            case 2:
                return '2';
            case 3:
                return '3';
            case 4:
                return '4';
            case 5:
                return '5';
            case 6:
                return '6';
            case 7:
                return '7';
            case 8:
                return '8';
            case 9:
                return '9';
            case 10:
                return 'a';
            case 11:
                return 'b';
            case 12:
                return 'c';
            case 13:
                return 'd';
            case 14:
                return 'e';
            case 15:
                return 'f';
            case 16:
                return 'g';
            case 17:
                return 'h';
            case 18:
                return 'i';
            case 19:
                return 'j';
            case 20:
                return 'k';
            case 21:
                return 'l';
            case 22:
                return 'm';
            case 23:
                return 'n';
            case 24:
                return 'o';
            case 25:
                return 'p';
            case 26:
                return 'q';
            case 27:
                return 'r';
            case 28:
                return 's';
            case 29:
                return 't';
            case 30:
                return 'u';
            case 31:
                return 'v';
            case 32:
                return 'w';
            default:
                return 'x';
        }
    }
}
