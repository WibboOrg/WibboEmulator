using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveFloorPlanModelEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Map = Packet.PopString().ToLower().TrimEnd('\r');
            int DoorX = Packet.PopInt();
            int DoorY = Packet.PopInt();
            int DoorDirection = Packet.PopInt();
            int WallThick = Packet.PopInt();
            int FloorThick = Packet.PopInt();
            int WallHeight = Packet.PopInt();

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (Room == null)
            {
                return;
            }

            if (!Room.CheckRights(Session, false))
            {
                return;
            }

            if (Room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("roomsell.error.8", Session.Langue));
                return;
            }

            char[] validLetters =
            {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g',
                'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', '\r'
            };

            if (Map.Length > 5776) //76x76
            {
                Session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "(%%%general%%%): %%%too_large_area%%% (%%%max%%% 5776 %%%tiles%%%)"));
                return;
            }

            Map = new Regex(@"[^a-z0-9\r]", RegexOptions.IgnoreCase).Replace(Map, string.Empty);

            if (string.IsNullOrEmpty(Map))
            {
                Session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entr� un Floormap invalide! (Map vide)"));
                return;
            }

            if (Map.Any(letter => !validLetters.Contains(letter)))
            {
                //Logging.LogException("Erreur map: " + Map);
                Session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entr� un Floormap invalide! (Code map)"));
                return;
            }

            string[] modelData = Map.Split('\r');

            int SizeY = modelData.Length;
            int SizeX = modelData[0].Length;

            if (SizeY > 75 || SizeX > 75 || SizeX < 1 || SizeY < 1)
            {
                Session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "La hauteur et la largeur maximales d'un modèle sont de 75x75!"));
                return;
            }

            bool isValid = true;

            for (int i = 0; i < modelData.Length; i++)
            {
                if (SizeX != modelData[i].Length)
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                Session.SendPacket(new RoomNotificationComposer("floorplan_editor.error", "errors", "Oups, il semble que vous avez entr� un Floormap invalide! (Forme)"));
                return;
            }

            int DoorZ = 0;

            try
            {
                DoorZ = parse(modelData[DoorY][DoorX]);
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomModelCustomDao.Replace(dbClient, Room.Id, DoorX, DoorY, DoorZ, DoorDirection, Map, WallHeight);
                dbClient.RunQuery("UPDATE rooms SET model_name = 'model_custom', wallthick = '" + WallThick + "', floorthick = '" + FloorThick + "' WHERE id = " + Room.Id + " LIMIT 1");
            }

            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);


            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                {
                    continue;
                }

                User.GetClient().SendPacket(new RoomForwardComposer(Room.Id));
            }
        }

        private static short parse(char input)
        {

            switch (input)
            {
                default:
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;
                case 'a':
                    return 10;
                case 'b':
                    return 11;
                case 'c':
                    return 12;
                case 'd':
                    return 13;
                case 'e':
                    return 14;
                case 'f':
                    return 15;
                case 'g':
                    return 16;
                case 'h':
                    return 17;
                case 'i':
                    return 18;
                case 'j':
                    return 19;
                case 'k':
                    return 20;
                case 'l':
                    return 21;
                case 'm':
                    return 22;
                case 'n':
                    return 23;
                case 'o':
                    return 24;
                case 'p':
                    return 25;
                case 'q':
                    return 26;
                case 'r':
                    return 27;
                case 's':
                    return 28;
                case 't':
                    return 29;
                case 'u':
                    return 30;
                case 'v':
                    return 31;
                case 'w':
                    return 32;
            }
        }
    }
}