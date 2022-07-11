using WibboEmulator.Game.Clients;
using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Camera
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

            if (!response.IsSuccessStatusCode)
                return;

            string photoId = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
            {
                return;
            }

            session.SendPacket(new ThumbnailStatusComposer(true, true));
        }
    }
}
