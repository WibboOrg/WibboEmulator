namespace WibboEmulator.Core;

public class UploadApi
{
    public static string ChatAudio(byte[] audioBinary, string audioName)
    {
        var content = new MultipartFormDataContent("Upload")
        {
            { new StreamContent(new MemoryStream(audioBinary)), "audio", audioName }
        };

        var audioUploadUrl = WibboEnvironment.GetSettings().GetData<string>("audio.upload.url");

        var response = WibboEnvironment.GetHttpClient().PostAsync(audioUploadUrl, content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var audioId = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        return audioId;
    }

    public static string CameraThubmail(byte[] photoBinary, string pictureName)
    {
        var content = new MultipartFormDataContent("Upload")
        {
            { new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName }
        };

        var response = WibboEnvironment.GetHttpClient().PostAsync(WibboEnvironment.GetSettings().GetData<string>("camera.thubmail.upload.url"), content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var photoId = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        return photoId;
    }

    public static string CameraPhoto(byte[] photoBinary, string pictureName)
    {
        var content = new MultipartFormDataContent("Upload")
        {
            { new StreamContent(new MemoryStream(photoBinary)), "photo", pictureName }
        };

        var response = WibboEnvironment.GetHttpClient().PostAsync(WibboEnvironment.GetSettings().GetData<string>("camera.upload.url"), content).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var photoId = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

        return photoId;
    }
}