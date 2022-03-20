using Butterfly.Game.Clients;
using System.Net.Http;
using System.IO;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Game.Rooms;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            int time = ButterflyEnvironment.GetUnixTimestamp();
            string pictureName = $"{session.GetUser().Id}_{room.Id}_{time}";

            MultipartFormDataContent content = new MultipartFormDataContent("Upload");
            content.Add(new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName);

            HttpResponseMessage response = await ButterflyEnvironment.GetHttpClient().PostAsync(ButterflyEnvironment.CameraUploadUrl, content);

            string photoId = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
            {
                session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.buyphoto.error", session.Langue) + " ( " + photoId + " ) ");
                return;
            }

            session.SendPacket(new CameraURLComposer(photoId + ".png"));

            if (session.GetUser().LastPhotoId == photoId)
            {
                return;
            }

            session.GetUser().LastPhotoId = photoId;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                UserPhotoDao.Insert(dbClient, session.GetUser().Id, photoId, time);
        }
    }
}
