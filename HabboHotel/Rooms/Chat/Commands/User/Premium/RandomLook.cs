using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class RandomLook : IChatCommand    {        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            //GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetRandomClient();
            //if (Client == null || Client.GetHabbo() == null)
            //return;

            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())            {                queryreactor.SetQuery("SELECT look FROM user_wardrobe WHERE user_id IN (SELECT user_id FROM (SELECT user_id FROM user_wardrobe WHERE user_id >= ROUND(RAND() * (SELECT max(user_id) FROM user_wardrobe)) LIMIT 1) tmp) ORDER BY RAND() LIMIT 1");                Session.GetHabbo().Look = queryreactor.GetString();            }            if (UserRoom.transformation || UserRoom.IsSpectator)
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;            if (currentRoom == null)
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom;            if (roomUserByHabbo == null)
            {
                return;
            }

            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));            currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));        }    }}