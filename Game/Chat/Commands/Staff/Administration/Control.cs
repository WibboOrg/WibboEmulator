using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Control : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];

            RoomUser roomUserByUserId = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
            {
                return;
            }

            if (Session.Langue != roomUserByUserId.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), Session.Langue));
                return;
            }

            Session.GetUser().ControlUserId = roomUserByUserId.GetClient().GetUser().Id;

        }
    }
}
