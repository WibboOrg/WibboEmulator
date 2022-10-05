namespace WibboEmulator.Communication.Packets.Incoming.Camera;
using WibboEmulator.Communication.Packets.Outgoing.Camera;
using WibboEmulator.Games.GameClients;

internal class RenderRoomThumbnailEvent : IPacketEvent
{
    public double Delay => 5000;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var photoLength = packet.PopInt();

        if (photoLength > 40000)
        {
            return;
        }

        var photoBinary = packet.ReadBytes(photoLength);

        if (session.GetUser() == null)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;
        if (room == null)
        {
            return;
        }

        var pictureName = $"thumbnail_{room.Id}";

        var content = new MultipartFormDataContent("Upload")
        {
            { new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName }
        };

        var response = WibboEnvironment.GetHttpClient().PostAsync(WibboEnvironment.GetSettings().GetData<string>("camera.thubmail.upload.url"), content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var photoId = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        if (string.IsNullOrEmpty(photoId) || pictureName != photoId)
        {
            return;
        }

        session.SendPacket(new ThumbnailStatusComposer(true, true));
    }
}
