using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
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

            RoomUser roomUserByUserId = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(username);
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
