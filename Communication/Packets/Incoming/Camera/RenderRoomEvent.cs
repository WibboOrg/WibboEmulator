namespace WibboEmulator.Communication.Packets.Incoming.Camera;
using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Core;
using WibboEmulator.Core.Language;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class RenderRoomEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        var photoLength = packet.PopInt();

        if (photoLength > 250_000)
        {
            return;
        }

        var photoBinary = packet.ReadBytes(photoLength);

        var room = session.User.Room;
        if (room == null)
        {
            return;
        }

        var time = WibboEnvironment.GetUnixTimestamp();
        var pictureName = $"{session.User.Id}_{room.Id}_{Guid.NewGuid()}";

        var photoId = UploadApi.CameraPhoto(photoBinary, pictureName);

        if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
        {
            session.SendNotification(LanguageManager.TryGetValue("notif.error", session.Language));
            return;
        }

        session.SendPacket(new CameraURLComposer(photoId));

        if (session.User.LastPhotoId == photoId)
        {
            return;
        }

        session.User.LastPhotoId = photoId;

        using var dbClient = DatabaseManager.Connection;
        UserPhotoDao.Insert(dbClient, session.User.Id, photoId, time);
    }
}
