namespace WibboEmulator.Communication.Packets.Incoming.Televisions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;

internal class EditTvYoutubeEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var ItemId = packet.PopInt();
        var Url = packet.PopString();

        if (session == null || session.GetUser() == null)
        {
            return;
        }

        var room = session.GetUser().CurrentRoom;
        if (room == null || !room.CheckRights(session))
        {
            return;
        }

        var item = room.GetRoomItemHandler().GetItem(ItemId);
        if (item == null || item.GetBaseItem().InteractionType != InteractionType.TVYOUTUBE)
        {
            return;
        }

        if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
        {            return;        }        var Split = "";        if (Url.Contains("?v="))        {            Split = Url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];        }        else if (Url.Contains("youtu.be/"))        {            Split = Url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];        }        if (Split.Length < 11)        {            return;        }        var VideoId = Split[..11];

        item.ExtraData = VideoId;
        item.UpdateState();
    }
}
