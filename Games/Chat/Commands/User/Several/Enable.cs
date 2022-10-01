using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms.Games;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class Enable : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (!int.TryParse(Params[1], out int NumEnable))
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, Session.GetUser().HasPermission("perm_god")))
            {
                return;
            }

            if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
            {
                return;
            }

            int CurrentEnable = UserRoom.CurrentEffect;
            if (CurrentEnable == 28 || CurrentEnable == 29 || CurrentEnable == 30 || CurrentEnable == 184)
            {
                return;
            }

            UserRoom.ApplyEffect(NumEnable);
        }
    }
}
