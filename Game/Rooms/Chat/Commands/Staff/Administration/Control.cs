using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class Control : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];

            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null)
            {
                return;
            }

            if (Session.Langue != roomUserByHabbo.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByHabbo.GetClient().Langue), Session.Langue));
                return;
            }

            Session.GetHabbo().ControlUserId = roomUserByHabbo.GetClient().GetHabbo().Id;
            UserRoom.SendWhisperChat("Tu contrôles " + roomUserByHabbo.GetClient().GetHabbo().Username() + " !");

        }
    }
}
