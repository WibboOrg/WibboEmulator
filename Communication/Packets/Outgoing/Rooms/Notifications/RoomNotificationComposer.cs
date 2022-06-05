namespace Wibbo.Communication.Packets.Outgoing.Rooms.Notifications
{
    internal class RoomNotificationComposer : ServerPacket
    {
        public RoomNotificationComposer(string Type, string Key, string Value)
           : base(ServerPacketHeader.NOTIFICATION_LIST)
        {
            this.WriteString(Type);
            this.WriteInteger((Type == "furni_placement_error") ? 2 : 1);//Count
            {
                if (Type == "furni_placement_error")
                {
                    this.WriteString("display");
                    this.WriteString("BUBBLE");
                }
                this.WriteString(Key);
                this.WriteString(Value);
            }
        }

        public static ServerPacket SendBubble(string Image, string Message, string linkUrl = "")
        {
            ServerPacket RoomNotification = new ServerPacket(ServerPacketHeader.NOTIFICATION_LIST);
            RoomNotification.WriteString(Image);
            RoomNotification.WriteInteger((string.IsNullOrWhiteSpace(linkUrl)) ? 2 : 3);//Count
            {
                RoomNotification.WriteString("display");
                RoomNotification.WriteString("BUBBLE");

                RoomNotification.WriteString("message");
                RoomNotification.WriteString(Message);

                if (!string.IsNullOrEmpty(linkUrl))
                {
                    RoomNotification.WriteString("linkUrl");
                    RoomNotification.WriteString(linkUrl);
                }
            }

            return RoomNotification;
        }

        public RoomNotificationComposer(string Title, string Message, string Image, string HotelName, string HotelURL)
            : base(ServerPacketHeader.NOTIFICATION_LIST)
        {
            int CountMessage = 2;
            if (!string.IsNullOrEmpty(HotelName))
            {
                CountMessage += 2;
            }

            this.WriteString(Image);
            this.WriteInteger(CountMessage);
            this.WriteString("title");
            this.WriteString(Title);
            this.WriteString("message");
            this.WriteString(Message);

            if (!string.IsNullOrEmpty(HotelName))
            {
                this.WriteString("linkUrl");
                this.WriteString(HotelURL);
                this.WriteString("linkTitle");
                this.WriteString(HotelName);
            }
        }
    }
}
