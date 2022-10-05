using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Camera
{
    internal class RenderRoomThumbnailEvent : IPacketEvent
    {
        public double Delay => 5000;

        public void Parse(GameClient session, ClientPacket packet)
        {
            int photoLength = packet.PopInt();

            if (photoLength > 40000)
                return;

            byte[] photoBinary = packet.ReadBytes(photoLength);

            if (session.GetUser() == null)
                return;

            Room room = session.GetUser().CurrentRoom;
            if (room == null)
                return;

            string pictureName = $"thumbnail_{room.Id}";

            MultipartFormDataContent content = new MultipartFormDataContent("Upload");
            content.Add(new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName);

            HttpResponseMessage response = WibboEnvironment.GetHttpClient().PostAsync(WibboEnvironment.GetSettings().GetData<string>("camera.thubmail.upload.url"), content).GetAwaiter().GetResult();

            if (!response.IsSuccessStatusCode)
                return;

            string photoId = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
            {
                return;
            }

            session.SendPacket(new ThumbnailStatusComposer(true, true));
        }
    }
}
