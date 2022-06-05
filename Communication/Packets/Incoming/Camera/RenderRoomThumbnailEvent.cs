using Wibbo.Game.Clients;
using Wibbo.Communication.Packets.Outgoing.Camera;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Camera
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

            int time = WibboEnvironment.GetUnixTimestamp();
            string pictureName = $"thumbnail_{room.Id}";

            MultipartFormDataContent content = new MultipartFormDataContent("Upload");
            content.Add(new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName);

            HttpResponseMessage response = await WibboEnvironment.GetHttpClient().PostAsync(WibboEnvironment.CameraThubmailUploadUrl, content);

            string photoId = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
            {
                return;
            }

            session.SendPacket(new ThumbnailStatusComposer(true, true));
        }
    }
}
