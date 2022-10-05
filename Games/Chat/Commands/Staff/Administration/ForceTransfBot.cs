namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceTransfBot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var username = Params[1];

        var roomUserByUserId = Room.GetRoomUserManager().GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
        {
            return;
        }

        if (session.Langue != roomUserByUserId.GetClient().Langue)
        {
            session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByUserId.GetClient().Langue), session.Langue));
            return;
        }

        if (!roomUserByUserId.IsTransf && !roomUserByUserId.IsSpectator)
        {
            var RoomClient = session.GetUser().CurrentRoom;
            if (RoomClient != null)
            {
                roomUserByUserId.TransfBot = !roomUserByUserId.TransfBot;

                RoomClient.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
                RoomClient.SendPacket(new UsersComposer(roomUserByUserId));
            }
        }

    }
}
