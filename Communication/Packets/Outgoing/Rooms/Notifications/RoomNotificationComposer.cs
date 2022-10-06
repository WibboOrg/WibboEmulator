namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Notifications;

internal class RoomNotificationComposer : ServerPacket
{
    public RoomNotificationComposer(string type, string key, string value)
       : base(ServerPacketHeader.NOTIFICATION_LIST)
    {
        this.WriteString(type);
        this.WriteInteger((type == "furni_placement_error") ? 2 : 1);//Count
        {
            if (type == "furni_placement_error")
            {
                this.WriteString("display");
                this.WriteString("BUBBLE");
            }
            this.WriteString(key);
            this.WriteString(value);
        }
    }

    public static ServerPacket SendBubble(string image, string message, string linkUrl = "")
    {
        var roomNotification = new ServerPacket(ServerPacketHeader.NOTIFICATION_LIST);
        roomNotification.WriteString(image);
        roomNotification.WriteInteger(string.IsNullOrWhiteSpace(linkUrl) ? 2 : 3);//Count
        {
            roomNotification.WriteString("display");
            roomNotification.WriteString("BUBBLE");

            roomNotification.WriteString("message");
            roomNotification.WriteString(message);

            if (!string.IsNullOrEmpty(linkUrl))
            {
                roomNotification.WriteString("linkUrl");
                roomNotification.WriteString(linkUrl);
            }
        }

        return roomNotification;
    }

    public RoomNotificationComposer(string title, string message, string image, string hotelName, string hotelURL)
        : base(ServerPacketHeader.NOTIFICATION_LIST)
    {
        var countMessage = 2;
        if (!string.IsNullOrEmpty(hotelName))
        {
            countMessage += 2;
        }

        this.WriteString(image);
        this.WriteInteger(countMessage);
        this.WriteString("title");
        this.WriteString(title);
        this.WriteString("message");
        this.WriteString(message);

        if (!string.IsNullOrEmpty(hotelName))
        {
            this.WriteString("linkUrl");
            this.WriteString(hotelURL);
            this.WriteString("linkTitle");
            this.WriteString(hotelName);
        }
    }
}
