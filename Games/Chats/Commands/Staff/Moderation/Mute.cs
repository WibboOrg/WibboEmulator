namespace WibboEmulator.Games.Chats.Commands.Staff.Moderation;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Mute : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);
        if (TargetUser == null || TargetUser.User == null)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("input.usernotfound", Session.Language));
        }
        else if (TargetUser.User.Rank >= Session.User.Rank)
        {
            userRoom.SendWhisperChat(LanguageManager.TryGetValue("action.notallowed", Session.Language));
        }
        else
        {
            var user = TargetUser.User;

            user.SpamProtectionTime = 300;
            user.SpamEnable = true;
            TargetUser.SendPacket(new FloodControlComposer(user.SpamProtectionTime));
        }
    }
}
