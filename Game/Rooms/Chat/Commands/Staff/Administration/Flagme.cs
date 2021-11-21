using Butterfly.Communication.Packets.Outgoing.Handshake;

using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Flagme : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;
            }

            clientByUsername.GetHabbo().CanChangeName = true;
            clientByUsername.SendPacket(new UserObjectComposer(clientByUsername.GetHabbo()));
            clientByUsername.SendWhisperChat("Tu peux maintenant changer ton pseudonyme");
        }
    }
}
