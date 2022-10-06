namespace WibboEmulator.Games.Chat.Commands.User.Build;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

internal class MaxFloor : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
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

    private static char ParseInverse(double input) => input switch
    {
        0 => '0',
        1 => '1',
        2 => '2',
        3 => '3',
        4 => '4',
        5 => '5',
        6 => '6',
        7 => '7',
        8 => '8',
        9 => '9',
        10 => 'a',
        11 => 'b',
        12 => 'c',
        13 => 'd',
        14 => 'e',
        15 => 'f',
        16 => 'g',
        17 => 'h',
        18 => 'i',
        19 => 'j',
        20 => 'k',
        21 => 'l',
        22 => 'm',
        23 => 'n',
        24 => 'o',
        25 => 'p',
        26 => 'q',
        27 => 'r',
        28 => 's',
        29 => 't',
        30 => 'u',
        31 => 'v',
        32 => 'w',
        _ => 'x',
    };
}
