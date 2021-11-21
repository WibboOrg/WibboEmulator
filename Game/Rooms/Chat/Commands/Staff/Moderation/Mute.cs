using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Game.GameClients;using Butterfly.Game.User;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class Mute : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)            {                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));            }            else            {                Habbo habbo = clientByUsername.GetHabbo();

                habbo.spamProtectionTime = 300;                habbo.spamEnable = true;                clientByUsername.SendPacket(new FloodControlComposer(habbo.spamProtectionTime));            }        }    }}