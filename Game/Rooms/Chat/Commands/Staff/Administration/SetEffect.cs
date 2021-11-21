using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class SetEffect : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            int.TryParse(Params[1], out int EnableNum);

            if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(EnableNum, Session.GetHabbo().HasFuse("fuse_sysadmin")))
            {
                return;
            }

            UserRoom.ApplyEffect(EnableNum);
        }
    }
}
