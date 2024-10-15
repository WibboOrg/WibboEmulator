namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Log;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal sealed partial class ChatAudioEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || !Session.User.InRoom || !Session.User.HasPermission("chat_audio") || Session.User.IgnoreAll)
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (user == null || user.IsSpectator)
        {
            return;
        }

        user.Unidle();

        if (!Session.User.HasPermission("no_mute") && !user.IsOwner && !room.CheckRights(Session) && room.RoomMuted)
        {
            user.SendWhisperChat(LanguageManager.TryGetValue("room.muted", Session.Language));
            return;
        }

        if (!Session.User.HasPermission("mod") && !user.IsOwner && !room.CheckRights(Session) && room.UserIsMuted(Session.User.Id))
        {
            if (!room.HasMuteExpired(Session.User.Id))
            {
                user.SendWhisperChat(LanguageManager.TryGetValue("user.muted", Session.Language));
                return;
            }
            else
            {
                room.RemoveMute(Session.User.Id);
            }
        }

        var timeSpan = DateTime.Now - Session.User.SpamFloodTime;
        if (timeSpan.TotalSeconds > Session.User.SpamProtectionTime && Session.User.SpamEnable)
        {
            Session.User.FloodCount = 0;
            Session.User.SpamEnable = false;
        }
        else if (timeSpan.TotalSeconds > 4.0)
        {
            Session.User.FloodCount = 0;
        }

        if (timeSpan.TotalSeconds < Session.User.SpamProtectionTime && Session.User.SpamEnable)
        {
            var secondsLeft = Session.User.SpamProtectionTime - timeSpan.Seconds;
            user.Client?.SendPacket(new FloodControlComposer(secondsLeft));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && Session.User.FloodCount > 5 && !Session.User.HasPermission("flood_chat"))
        {
            Session.User.SpamProtectionTime = room.IsRoleplay || Session.User.HasPermission("flood_premium") ? 5 : 15;
            Session.User.SpamEnable = true;

            user.Client?.SendPacket(new FloodControlComposer(Session.User.SpamProtectionTime - timeSpan.Seconds));

            return;
        }

        Session.User.SpamFloodTime = DateTime.Now;
        Session.User.FloodCount++;

        var audioLength = packet.PopInt();

        if (audioLength > 250_000)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.error", Session.Language));
            return;
        }

        var audioBinary = packet.ReadBytes(audioLength);

        var audioName = $"{Session.User.Id}_{room.Id}_{Guid.NewGuid()}";

        var audioId = UploadApi.ChatAudio(audioBinary, audioName);

        if (string.IsNullOrEmpty(audioId) || audioName != audioId)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.error", Session.Language));
            return;
        }

        var audioPath = $"/chat-audio/{audioName}.webm";

        var audioUploadUrl = SettingsManager.GetData<string>("audio.upload.url");
        var basePath = new Uri(audioUploadUrl).GetLeftPart(UriPartial.Authority);

        var audioUrl = $"{basePath}{audioPath}";

        Session.User.ChatMessageManager.AddMessage(Session.User.Id, Session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());
        room.ChatlogManager.AddMessage(Session.User.Id, Session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());

        using var dbClient = DatabaseManager.Connection;
        LogChatDao.Insert(dbClient, Session.User.Id, room.Id, audioUrl, "audio", Session.User.Username);

        user.OnChatAudio(audioPath);
    }
}
