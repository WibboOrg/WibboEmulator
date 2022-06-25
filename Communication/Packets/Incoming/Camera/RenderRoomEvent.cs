using WibboEmulator.Game.Clients;
using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;

namespace WibboEmulator.Communication.Packets.Incoming.Camera
{
    internal class RenderRoomEvent : IPacketEvent
    {
        public double Delay => 5000;

        public async void Parse(Client session, ClientPacket packet)
        {
            int photoLength = packet.PopInt();
            byte[] photoBinary = packet.ReadBytes(photoLength);

            if (session.GetUser() == null)
                return;

            Room room = session.GetUser().CurrentRoom;
            if (room == null)
                return;

            int time = WibboEnvironment.GetUnixTimestamp();
            string pictureName = $"{session.GetUser().Id}_{room.Id}_{time}";

            MultipartFormDataContent content = new MultipartFormDataContent("Upload");
            content.Add(new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName);

            HttpResponseMessage response = await WibboEnvironment.GetHttpClient().PostAsync(WibboEnvironment.CameraUploadUrl, content);

            string photoId = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
            {
                session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue));
                return;
            }

            session.SendPacket(new CameraURLComposer(photoId + ".png"));

            if (session.GetUser().LastPhotoId == photoId)
            {
                return;
            }

            session.GetUser().LastPhotoId = photoId;

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                UserPhotoDao.Insert(dbClient, session.GetUser().Id, photoId, time);
        }
    }
}
