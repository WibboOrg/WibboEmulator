using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class ForceTransfBot : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];

            RoomUser roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByName(username);
            if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
            {
                return;
            }

            if (Session.Langue != roomUserByUserId.GetClient().Langue)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), Session.Langue));
                return;
            }

            if (!roomUserByUserId.IsTransf && !roomUserByUserId.IsSpectator)
            {
                Room RoomClient = Session.GetUser().CurrentRoom;
                if (RoomClient != null)
                {
                    roomUserByUserId.TransfBot = !roomUserByUserId.TransfBot;

                    RoomClient.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
                    RoomClient.SendPacket(new UsersComposer(roomUserByUserId));
                }
            }

        }
    }
}
