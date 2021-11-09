using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Enable : IChatCommand    {        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (Params.Length != 2)
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

            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            int CurrentEnable = UserRoom.CurrentEffect;            if (CurrentEnable == 28 || CurrentEnable == 29 || CurrentEnable == 30 || CurrentEnable == 184)
            {
                return;
            }

            UserRoom.ApplyEffect(NumEnable);        }    }}