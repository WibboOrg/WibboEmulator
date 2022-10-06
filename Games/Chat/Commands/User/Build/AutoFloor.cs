namespace WibboEmulator.Games.Chat.Commands.User.Build;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

internal class AutoFloor : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var Map = "";
        for (var y = 0; y < Room.GetGameMap().Model.MapSizeY; y++)
        {
            var Line = "";
            for (var x = 0; x < Room.GetGameMap().Model.MapSizeX; x++)
            {
                if (x >= Room.GetGameMap().Model.MapSizeX || y >= Room.GetGameMap().Model.MapSizeY)
                {
                    Line += "x";
                }
                else
                {
                    if (Room.GetGameMap().Model.SqState[x, y] == SquareStateType.BLOCKED || Room.GetGameMap().GetCoordinatedItems(new Point(x, y)).Count == 0)
                    {
                        Line += "x";//x
                    }
                    else
                    {
                        Line += ParseInvers(Room.GetGameMap().Model.SqFloorHeight[x, y]);
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

    private static char ParseInvers(double input) => input switch
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
