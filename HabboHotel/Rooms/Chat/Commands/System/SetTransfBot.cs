using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class SetTransfBot : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null)
            {
                return;
            }

            if (Session.Langue != roomUserByHabbo.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByHabbo.GetClient().Langue), Session.Langue));
                return;
            }            if (!roomUserByHabbo.transformation && !roomUserByHabbo.IsSpectator)            {                Room RoomClient = Session.GetHabbo().CurrentRoom;                if (RoomClient != null)                {                    roomUserByHabbo.transfbot = !roomUserByHabbo.transfbot;                    RoomClient.SendPacket(new UserRemoveComposer(roomUserByHabbo.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(roomUserByHabbo));
                }            }        }    }}