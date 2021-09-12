using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class SetEnable : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (Params.Length != 3)
            {
                return;
            }

            string Username = Params[1];            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(Username);            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null)
            {
                return;
            }

            if (Session.Langue != roomUserByHabbo.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByHabbo.GetClient().Langue), Session.Langue));
                return;
            }            if (!int.TryParse(Params[2], out int NumEnable))
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(NumEnable, Session.GetHabbo().HasFuse("fuse_sysadmin")))
            {
                return;
            }

            roomUserByHabbo.ApplyEffect(NumEnable);        }    }}