using Butterfly.Game.Clients;
using System.Net.Http;
using System.IO;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Game.Rooms;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;

namespace Butterfly.Communication.Packets.Incoming.Camera
{
    internal class RenderRoomThumbnailEvent : IPacketEvent
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
            string pictureName = $"thumbnail_{room.Id}";

            MultipartFormDataContent content = new MultipartFormDataContent("Upload");
            content.Add(new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName);

            HttpResponseMessage response = await ButterflyEnvironment.GetHttpClient().PostAsync(ButterflyEnvironment.CameraThubmailUploadUrl, content);

            string photoId = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
            {
                return;
            }

            session.SendPacket(new ThumbnailStatusComposer(true, true));
        }
    }
}
