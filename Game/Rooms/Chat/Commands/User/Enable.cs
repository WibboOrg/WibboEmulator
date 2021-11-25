using Butterfly.Game.Clients;using Butterfly.Game.Rooms.Games;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class Enable : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            if (!int.TryParse(Params[1], out int NumEnable))
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, Session.GetHabbo().HasFuse("fuse_sysadmin")))
            {
                return;
            }

            if (UserRoom.Team != TeamType.none || UserRoom.InGame)
            {
                return;
            }

            int CurrentEnable = UserRoom.CurrentEffect;            if (CurrentEnable == 28 || CurrentEnable == 29 || CurrentEnable == 30 || CurrentEnable == 184)
            {
                return;
            }

            UserRoom.ApplyEffect(NumEnable);        }    }}