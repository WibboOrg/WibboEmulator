namespace WibboEmulator.Communication.Packets.Incoming.Rooms.FloorPlan;
using System.Text.RegularExpressions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.session;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class SaveFloorPlanModelEvent : IPacketEvent
{
    public double Delay => 1000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var Map = packet.PopString().ToLower().TrimEnd('\r');
        var DoorX = packet.PopInt();
        var DoorY = packet.PopInt();
        var DoorDirection = packet.PopInt();
        var WallThick = packet.PopInt();
        var FloorThick = packet.PopInt();
        var WallHeight = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, false))
        {
            return;
        }

        if (room.RoomData.SellPrice > 0)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("roomsell.error.8", session.Langue));
            return;
        }

        char[] validLetters =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
            'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', '\r'
        };

        if (Map.Length > 5776) //76x76
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "(%%%general%%%): %%%too_large_area%%% (%%%max%%% 5776 %%%tiles%%%)"));
            return;
        }

        Map = new Regex(@"[^a-z0-9\r]", RegexOptions.IgnoreCase).Replace(Map, string.Empty);

        if (string.IsNullOrEmpty(Map))
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entré un Floormap invalide! (Map vide)"));
            return;
        }

        if (Map.Any(letter => !validLetters.Contains(letter)))
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entré un Floormap invalide! (Code map)"));
            return;
        }

        var modelData = Map.Split('\r');

        var SizeY = modelData.Length;
        var SizeX = modelData[0].Length;

        if (SizeY > 75 || SizeX > 75 || SizeX < 1 || SizeY < 1)
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "La hauteur et la largeur maximales d'un modèle sont de 75x75!"));
            return;
        }

        var isValid = true;

        for (var i = 0; i < modelData.Length; i++)
        {
            if (SizeX != modelData[i].Length)
            {
                isValid = false;
            }
        }

        if (!isValid)
        {
            session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entré un Floormap invalide! (Forme)"));
            return;
        }

        var DoorZ = 0;

        try
        {
            DoorZ = Parse(modelData[DoorY][DoorX]);
        }
        catch { }

        if (WallThick > 1)
        {
            WallThick = 1;
        }

        if (WallThick < -2)
        {
            WallThick = -2;
        }

        if (FloorThick > 1)
        {
            FloorThick = 1;
        }

        if (FloorThick < -2)
        {
            WallThick = -2;
        }

        if (WallHeight < 0)
        {
            WallHeight = 0;
        }

        if (WallHeight > 15)
        {
            WallHeight = 15;
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomModelCustomDao.Replace(dbClient, room.Id, DoorX, DoorY, DoorZ, DoorDirection, Map, WallHeight);
            RoomDao.UpdateModelWallThickFloorThick(dbClient, room.Id, WallThick, FloorThick);
        }

        var UsersToReturn = room.GetRoomUserManager().GetRoomUsers().ToList();

        WibboEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

        foreach (var User in UsersToReturn)
        {
            if (User == null || User.GetClient() == null)
            {
                continue;
            }

            User.GetClient().SendPacket(new RoomForwardComposer(room.Id));
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