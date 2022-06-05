using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class ForceEnableUser : IChatCommand
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
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), Session.Langue));
                return;
            }

            if (!int.TryParse(Params[2], out int NumEnable))
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, Session.GetUser().HasFuse("fuse_fullenable")))
            {
                return;
            }

            roomUserByUserId.ApplyEffect(NumEnable);
        }
    }
}
