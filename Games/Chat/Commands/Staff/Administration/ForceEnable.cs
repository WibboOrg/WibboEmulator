using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ForceEnable : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            int.TryParse(Params[1], out int EnableNum);

            if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(EnableNum, Session.GetUser().HasPermission("perm_god")))
            {
                return;
            }

            UserRoom.ApplyEffect(EnableNum);
        }
    }
}
