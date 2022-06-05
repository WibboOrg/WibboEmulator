using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class RoomEnable : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!int.TryParse(Params[1], out int NumEnable))
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, Session.GetUser().HasFuse("fuse_fullenable")))
            {
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())
            {
                if (!User.IsBot)
                {
                    User.ApplyEffect(NumEnable);
                }
            }

        }
    }
}
