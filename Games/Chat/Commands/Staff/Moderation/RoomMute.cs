namespace WibboEmulator.Games.Chat.Commands.Staff.Moderation;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomMute : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        Room.RoomMuted = !Room.RoomMuted;

        foreach (var User in Room.GetRoomUserManager().GetRoomUsers())
        {
            if (User == null)
            {
                continue;
            }

            User.SendWhisperChat(Room.RoomMuted ? "Vous ne pouvez plus parler" : "Vous pouvez parler");
        }
    }
}
