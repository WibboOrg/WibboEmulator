using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class ForceTransfBot : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
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
