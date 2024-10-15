namespace WibboEmulator.Games.Chats.Commands.User.Build;

using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class SetZStop : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        userRoom.BuildToolStackHeight = false;
        Session.SendPacket(room.GameMap.Model.SerializeRelativeHeightmap());

        Session.SendWhisper(LanguageManager.TryGetValue("cmd.setz.disabled", Session.Language));
    }
}
