using Butterfly.Game.GameClients;namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class KickStaff : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            Room currentRoom = Session.GetHabbo().CurrentRoom;            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetHabbo().Rank <= clientByUsername.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else if (clientByUsername.GetHabbo().CurrentRoomId < 1U)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.error", Session.Langue));            }            else            {                Room.GetRoomUserManager().RemoveUserFromRoom(clientByUsername, true, false);                if (Params.Length > 2)                {                    string message = CommandManager.MergeParams(Params, 2);                    if (Session.Antipub(message, "<CMD>", Room.Id))
                    {
                        return;
                    }

                    clientByUsername.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.withmessage", clientByUsername.Langue) + message);                }                else
                {
                    clientByUsername.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.nomessage", clientByUsername.Langue));
                }
            }        }    }}