namespace WibboEmulator.Communication.Packets.Incoming.Camera;
using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal sealed class RenderRoomEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var photoLength = packet.PopInt();

        if (photoLength > 250000)
        {
            return;
        }

        var photoBinary = packet.ReadBytes(photoLength);

        if (session.User == null)
        {
            return;
        }

        var room = session.User.CurrentRoom;
        if (room == null)
        {
            return;
        }

        var time = WibboEnvironment.GetUnixTimestamp();
        var pictureName = $"{session.User.Id}_{room.Id}_{time}";

        var content = new MultipartFormDataContent("Upload")
        {
            { new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName }
        };

        var response = WibboEnvironment.GetHttpClient().PostAsync(WibboEnvironment.GetSettings().GetData<string>("camera.upload.url"), content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue));
            return;
        }

        var photoId = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
        {
            session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue));
            return;
        }

        session.SendPacket(new CameraURLComposer(photoId + ".png"));

        if (session.User.LastPhotoId == photoId)
        {
            return;
        }

        session.User.LastPhotoId = photoId;

        using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
        UserPhotoDao.Insert(dbClient, session.User.Id, photoId, time);
    }
}
