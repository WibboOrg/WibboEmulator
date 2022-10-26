namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class TransfBig : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (userRoom.Team != TeamType.None || userRoom.InGame)
        {
            return;
        }

        if (session.User.SpectatorMode)
        {
            return;
        }

        if (!userRoom.SetPetTransformation("big" + parameters[1], 0))
        {
            session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.littleorbig.help", session.Langue));
            return;
        }

        userRoom.IsTransf = true;

        room.SendPacket(new UserRemoveComposer(userRoom.VirtualId));
        room.SendPacket(new UsersComposer(userRoom));
    }
}
