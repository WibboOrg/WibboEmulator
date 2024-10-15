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

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || !session.User.InRoom || !session.User.HasPermission("chat_audio") || session.User.IgnoreAll)
        {
            return;
        }

        var room = session.User.Room;
        if (room == null)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null || user.IsSpectator)
        {
            return;
        }

        user.Unidle();

        if (!session.User.HasPermission("no_mute") && !user.IsOwner && !room.CheckRights(session) && room.RoomMuted)
        {
            user.SendWhisperChat(LanguageManager.TryGetValue("room.muted", session.Language));
            return;
        }

        if (!session.User.HasPermission("mod") && !user.IsOwner && !room.CheckRights(session) && room.UserIsMuted(session.User.Id))
        {
            if (!room.HasMuteExpired(session.User.Id))
            {
                user.SendWhisperChat(LanguageManager.TryGetValue("user.muted", session.Language));
                return;
            }
            else
            {
                room.RemoveMute(session.User.Id);
            }
        }

        var timeSpan = DateTime.Now - session.User.SpamFloodTime;
        if (timeSpan.TotalSeconds > session.User.SpamProtectionTime && session.User.SpamEnable)
        {
            session.User.FloodCount = 0;
            session.User.SpamEnable = false;
        }
        else if (timeSpan.TotalSeconds > 4.0)
        {
            session.User.FloodCount = 0;
        }

        if (timeSpan.TotalSeconds < session.User.SpamProtectionTime && session.User.SpamEnable)
        {
            var secondsLeft = session.User.SpamProtectionTime - timeSpan.Seconds;
            user.Client?.SendPacket(new FloodControlComposer(secondsLeft));
            return;
        }
        else if (timeSpan.TotalSeconds < 4.0 && session.User.FloodCount > 5 && !session.User.HasPermission("flood_chat"))
        {
            session.User.SpamProtectionTime = room.IsRoleplay || session.User.HasPermission("flood_premium") ? 5 : 15;
            session.User.SpamEnable = true;

            user.Client?.SendPacket(new FloodControlComposer(session.User.SpamProtectionTime - timeSpan.Seconds));

            return;
        }

        session.User.SpamFloodTime = DateTime.Now;
        session.User.FloodCount++;

        var audioLength = packet.PopInt();

        if (audioLength > 250_000)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.error", session.Language));
            return;
        }

        var audioBinary = packet.ReadBytes(audioLength);

        var audioName = $"{session.User.Id}_{room.Id}_{Guid.NewGuid()}";

        var audioId = UploadApi.ChatAudio(audioBinary, audioName);

        if (string.IsNullOrEmpty(audioId) || audioName != audioId)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.error", session.Language));
            return;
        }

        var audioPath = $"/chat-audio/{audioName}.webm";

        var audioUploadUrl = SettingsManager.GetData<string>("audio.upload.url");
        var basePath = new Uri(audioUploadUrl).GetLeftPart(UriPartial.Authority);

        var audioUrl = $"{basePath}{audioPath}";

        session.User.ChatMessageManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());
        room.ChatlogManager.AddMessage(session.User.Id, session.User.Username, room.Id, audioUrl, UnixTimestamp.GetNow());

        using var dbClient = DatabaseManager.Connection;
        LogChatDao.Insert(dbClient, session.User.Id, room.Id, audioUrl, "audio", session.User.Username);

        user.OnChatAudio(audioPath);
    }
}
