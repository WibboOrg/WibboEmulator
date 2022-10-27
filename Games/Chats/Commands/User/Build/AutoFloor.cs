namespace WibboEmulator.Games.Chats.Commands.User.Build;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Map;

internal class AutoFloor : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var map = "";
        for (var y = 0; y < room.GameMap.Model.MapSizeY; y++)
        {
            var line = "";
            for (var x = 0; x < room.GameMap.Model.MapSizeX; x++)
            {
                if (x >= room.GameMap.Model.MapSizeX || y >= room.GameMap.Model.MapSizeY)
                {
                    line += "x";
                }
                else
                {
                    if (room.GameMap.Model.SqState[x, y] == SquareStateType.Bloked || room.GameMap.GetCoordinatedItems(new Point(x, y)).Count == 0)
                    {
                        line += "x";
                    }
                    else
                    {
                        line += ParseInvers(room.GameMap.Model.SqFloorHeight[x, y]);
                    }
                }
            }
            map += line + Convert.ToChar(13);
        }

        map = map.TrimEnd(Convert.ToChar(13));

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomModelCustomDao.Replace(dbClient, room.Id, room.GameMap.Model.DoorX, room.GameMap.Model.DoorY, room.GameMap.Model.DoorZ, room.GameMap.Model.DoorOrientation, map, room.GameMap.Model.WallHeight);
            RoomDao.UpdateModel(dbClient, room.Id);
        }

        var usersToReturn = room.RoomUserManager.GetRoomUsers().ToList();

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        foreach (var user in usersToReturn)
        {
            if (user == null || user.Client == null)
            {
                continue;
            }

            user.Client.SendPacket(new RoomForwardComposer(room.Id));
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
