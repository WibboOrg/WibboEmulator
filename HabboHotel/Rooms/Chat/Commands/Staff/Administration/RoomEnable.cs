using Butterfly.HabboHotel.GameClients;using System.Linq;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class RoomEnable : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (!int.TryParse(Params[1], out int NumEnable))
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, Session.GetHabbo().HasFuse("fuse_sysadmin")))
            {
                return;
            }

            foreach (RoomUser User in Room.GetRoomUserManager().GetUserList().ToList())            {
                if (!User.IsBot)
                {
                    User.ApplyEffect(NumEnable);
                }            }        }    }}