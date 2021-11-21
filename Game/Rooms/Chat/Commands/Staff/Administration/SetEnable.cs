using Butterfly.Game.Clients;namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class SetEnable : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 3)
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