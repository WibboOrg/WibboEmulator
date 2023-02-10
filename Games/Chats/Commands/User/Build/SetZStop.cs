namespace WibboEmulator.Games.Chats.Commands.User.Build;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SetZStop : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        userRoom.ConstruitZMode = false;
        session.SendPacket(room.GameMap.Model.SerializeRelativeHeightmap());

        session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.setz.disabled", session.Langue));
    }
}
