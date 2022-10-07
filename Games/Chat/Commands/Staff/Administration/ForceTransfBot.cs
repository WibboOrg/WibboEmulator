namespace WibboEmulator.Games.Chat.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ForceTransfBot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = room.GetRoomUserManager().GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.GetClient() == null)
        {
            return;
        }

        if (session.Langue != roomUserByUserId.GetClient().Langue)
        {
            session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue.user", session.Langue), roomUserByUserId.GetClient().Langue));
            return;
        }

        if (!roomUserByUserId.IsTransf && !roomUserByUserId.IsSpectator)
        {
            roomUserByUserId.TransfBot = !roomUserByUserId.TransfBot;

            room.SendPacket(new UserRemoveComposer(roomUserByUserId.VirtualId));
            room.SendPacket(new UsersComposer(roomUserByUserId));
        }

    }
}
