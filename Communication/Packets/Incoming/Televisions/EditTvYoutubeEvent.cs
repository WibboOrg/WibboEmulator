namespace WibboEmulator.Communication.Packets.Incoming.Televisions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal sealed class EditTvYoutubeEvent : IPacketEvent
{
    public double Delay => 500;

    internal static readonly string[] Separator = ["?v="];
    internal static readonly string[] SeparatorArray = ["youtu.be/"];

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var url = packet.PopString();

        if (session == null || session.User == null)
        {
            return;
        }

        var room = session.User.Room;
        if (room == null || !room.CheckRights(session))
        {
            return;
        }

        var item = room.RoomItemHandling.GetItem(itemId);
        if (item == null || item.ItemData.InteractionType != InteractionType.TV_YOUTUBE)
        {
            return;
        }

        if (string.IsNullOrEmpty(url) || (!url.Contains("?v=") && !url.Contains("youtu.be/")))
        {
            return;
        }

        var split = "";

        if (url.Contains("?v="))
        {
            split = url.Split(Separator, StringSplitOptions.None)[1];
        }
        else if (url.Contains("youtu.be/"))
        {
            split = url.Split(SeparatorArray, StringSplitOptions.None)[1];
        }

        if (split.Length < 11)
        {
            return;
        }

        var videoId = split[..11];

        item.ExtraData = videoId;
        item.UpdateState();
    }
}
