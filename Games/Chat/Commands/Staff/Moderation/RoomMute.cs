namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomMute : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        room.RoomMuted = !room.RoomMuted;

        foreach (var user in room.GetRoomUserManager().GetRoomUsers())
        {
            if (user == null)
            {
                continue;
            }

            user.SendWhisperChat(room.RoomMuted ? "Vous ne pouvez plus parler" : "Vous pouvez parler");
        }
    }
}
