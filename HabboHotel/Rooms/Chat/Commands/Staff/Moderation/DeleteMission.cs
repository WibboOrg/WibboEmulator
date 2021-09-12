using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;

using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class DeleteMission : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(username);            if (clientByUsername == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetHabbo().Rank <= clientByUsername.GetHabbo().Rank)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.notpermitted", Session.Langue));            }            else            {                clientByUsername.GetHabbo().Motto = ButterflyEnvironment.GetLanguageManager().TryGetValue("user.unacceptable_motto", clientByUsername.Langue);                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())                {                    queryreactor.SetQuery("UPDATE users SET motto = @motto WHERE id = '" + clientByUsername.GetHabbo().Id + "'");                    queryreactor.AddParameter("motto", clientByUsername.GetHabbo().Motto);                    queryreactor.RunQuery();                }                Room currentRoom2 = clientByUsername.GetHabbo().CurrentRoom;                if (currentRoom2 == null)
                {
                    return;
                }

                RoomUser roomUserByHabbo = currentRoom2.GetRoomUserManager().GetRoomUserByHabboId(clientByUsername.GetHabbo().Id);                if (roomUserByHabbo == null)
                {
                    return;
                }

                currentRoom2.SendPacket(new UserChangeComposer(roomUserByHabbo, false));            }        }    }}