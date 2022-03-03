using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class ForceEnabled : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 3)
            {
                return;
            }

            string Username = Params[1];

            RoomUser roomUserByUserId = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(Username);
            if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
            {
                return;
            }

            if (Session.Langue != roomUserByUserId.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), Session.Langue));
                return;
            }

            if (!int.TryParse(Params[2], out int NumEnable))
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, Session.GetUser().HasFuse("fuse_enablefull")))
            {
                return;
            }

            roomUserByUserId.ApplyEffect(NumEnable);
        }
    }
}
