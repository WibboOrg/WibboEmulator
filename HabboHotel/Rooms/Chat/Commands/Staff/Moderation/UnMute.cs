using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Users;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class UnMute : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));            }            else            {                Habbo habboByUsername = clientByUsername.GetHabbo();

                habboByUsername.spamProtectionTime = 10;                habboByUsername.spamEnable = true;            }        }    }}