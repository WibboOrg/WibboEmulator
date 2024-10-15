namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal sealed class TransfBig : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (userRoom.Team != TeamType.None || userRoom.InGame || room.IsGameMode)
        {
            return;
        }

        if (Session.User.IsSpectator)
        {
            return;
        }

        if (!userRoom.SetPetTransformation("big" + parameters[1], 0))
        {
            Session.SendHugeNotification(LanguageManager.TryGetValue("cmd.littleorbig.help", Session.Language));
            return;
        }

        userRoom.IsTransf = true;

        room.SendPacket(new UserRemoveComposer(userRoom.VirtualId));
        room.SendPacket(new UsersComposer(userRoom));
    }
}
