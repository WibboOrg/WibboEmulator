namespace WibboEmulator.Communication.Packets.Incoming.Televisions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class EditTvYoutubeEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var url = packet.PopString();

        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;
        if (room == null || !room.CheckRights(session))
        {
            return;
        }

        var item = room.GetRoomItemHandler().GetItem(itemId);
        if (item == null || item.GetBaseItem().InteractionType != InteractionType.TVYOUTUBE)
        {
            return;
        }

        if (string.IsNullOrEmpty(url) || (!url.Contains("?v=") && !url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
        {            return;        }        var split = "";        if (url.Contains("?v="))        {            split = url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];        }        else if (url.Contains("youtu.be/"))        {            split = url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];        }        if (split.Length < 11)        {            return;        }        var videoId = split[..11];

        item.ExtraData = videoId;
        item.UpdateState();
    }
}
