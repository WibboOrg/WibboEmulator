namespace WibboEmulator.Communication.Packets.Incoming.Rooms.FloorPlan;
using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class SaveFloorPlanModelEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var map = packet.PopString().ToLower().TrimEnd('\r');
        var doorX = packet.PopInt();
        var doorY = packet.PopInt();
        var doorDirection = packet.PopInt();
        var wallThick = packet.PopInt();
        var floorThick = packet.PopInt();
        var wallHeight = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, false))
        {
            return;
        }

        if (room.Data.SellPrice > 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.8", session.Langue));
            return;
        }

        char[] validLetters =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
            'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', '\r'
        };

        if (map.Length > 5776) //76x76
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "(%%%general%%%): %%%too_large_area%%% (%%%max%%% 5776 %%%tiles%%%)"));
            return;
        }

        map = new Regex(@"[^a-z0-9\r]", RegexOptions.IgnoreCase).Replace(map, string.Empty);

        if (string.IsNullOrEmpty(map))
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entré un Floormap invalide! (Map vide)"));
            return;
        }

        if (map.Any(letter => !validLetters.Contains(letter)))
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entré un Floormap invalide! (Code map)"));
            return;
        }

        var modelData = map.Split('\r');

        var sizeY = modelData.Length;
        var sizeX = modelData[0].Length;

        if (sizeY > 75 || sizeX > 75 || sizeX < 1 || sizeY < 1)
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "La hauteur et la largeur maximales d'un modèle sont de 75x75!"));
            return;
        }

        var isValid = true;

        for (var i = 0; i < modelData.Length; i++)
        {
            if (sizeX != modelData[i].Length)
            {
                isValid = false;
            }
        }

        if (!isValid)
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entré un Floormap invalide! (Forme)"));
            return;
        }

        var doorZ = 0;

        try
        {
            doorZ = Parse(modelData[doorY][doorX]);
        }
        catch { }

        if (wallThick > 1)
        {
            wallThick = 1;
        }

        if (wallThick < -2)
        {
            wallThick = -2;
        }

        if (floorThick > 1)
        {
            floorThick = 1;
        }

        if (floorThick < -2)
        {
            wallThick = -2;
        }

        if (wallHeight < 0)
        {
            wallHeight = 0;
        }

        if (wallHeight > 15)
        {
            wallHeight = 15;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomModelCustomDao.Replace(dbClient, room.Id, doorX, doorY, doorZ, doorDirection, map, wallHeight);
            RoomDao.UpdateModelWallThickFloorThick(dbClient, room.Id, wallThick, floorThick);
        }

        var usersToReturn = room.GetRoomUserManager().GetRoomUsers().ToList();

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

    private static short Parse(char input) => input switch
    {
        '1' => 1,
        '2' => 2,
        '3' => 3,
        '4' => 4,
        '5' => 5,
        '6' => 6,
        '7' => 7,
        '8' => 8,
        '9' => 9,
        'a' => 10,
        'b' => 11,
        'c' => 12,
        'd' => 13,
        'e' => 14,
        'f' => 15,
        'g' => 16,
        'h' => 17,
        'i' => 18,
        'j' => 19,
        'k' => 20,
        'l' => 21,
        'm' => 22,
        'n' => 23,
        'o' => 24,
        'p' => 25,
        'q' => 26,
        'r' => 27,
        's' => 28,
        't' => 29,
        'u' => 30,
        'v' => 31,
        'w' => 32,
        _ => 0,
    };
}
