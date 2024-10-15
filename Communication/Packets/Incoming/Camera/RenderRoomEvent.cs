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

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        var photoLength = packet.PopInt();

        if (photoLength > 250_000)
        {
            return;
        }

        var photoBinary = packet.ReadBytes(photoLength);

        var room = Session.User.Room;
        if (room == null)
        {
            return;
        }

        var time = WibboEnvironment.GetUnixTimestamp();
        var pictureName = $"{Session.User.Id}_{room.Id}_{Guid.NewGuid()}";

        var photoId = UploadApi.CameraPhoto(photoBinary, pictureName);

        if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
        {
            Session.SendNotification(LanguageManager.TryGetValue("notif.error", Session.Language));
            return;
        }

        Session.SendPacket(new CameraURLComposer(photoId));

        if (Session.User.LastPhotoId == photoId)
        {
            return;
        }

        Session.User.LastPhotoId = photoId;

        using var dbClient = DatabaseManager.Connection;
        UserPhotoDao.Insert(dbClient, Session.User.Id, photoId, time);
    }
}
