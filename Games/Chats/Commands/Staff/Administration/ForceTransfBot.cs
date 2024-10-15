namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ForceTransfBot : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var username = parameters[1];

        var roomUserByUserId = room.RoomUserManager.GetRoomUserByName(username);
        if (roomUserByUserId == null || roomUserByUserId.Client == null)
        {
            return;
        }

        if (Session.Language != roomUserByUserId.Client.Language)
        {
            Session.SendWhisper(string.Format(LanguageManager.TryGetValue("cmd.authorized.langue.user", Session.Language), roomUserByUserId.Client.Language));
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
