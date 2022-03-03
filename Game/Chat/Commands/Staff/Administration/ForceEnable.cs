using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class ForceEnable : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            int.TryParse(Params[1], out int EnableNum);

            if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(EnableNum, Session.GetUser().HasFuse("fuse_sysadmin")))
            {
                return;
            }

            UserRoom.ApplyEffect(EnableNum);
        }
    }
}
