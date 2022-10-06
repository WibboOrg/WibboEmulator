namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.Games.Teams;

internal class TransfLittle : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        if (UserRoom.Team != TeamType.NONE || UserRoom.InGame)
        {
            return;
        }

        if (session.GetUser().SpectatorMode || UserRoom.InGame)
        {
            return;
        }

        if (!UserRoom.SetPetTransformation("little" + parameters[1], 0))
        {
            session.SendHugeNotif(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.littleorbig.help", session.Langue));
            return;
        }

        UserRoom.IsTransf = true;

        Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
        Room.SendPacket(new UsersComposer(UserRoom));
    }
}
