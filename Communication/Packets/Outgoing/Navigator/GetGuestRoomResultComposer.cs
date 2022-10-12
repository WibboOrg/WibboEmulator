namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GetGuestRoomResultComposer : ServerPacket
{
    public GetGuestRoomResultComposer(GameClient session, RoomData data, bool isLoading, bool checkEntry)
        : base(ServerPacketHeader.ROOM_INFO)
    {
        this.WriteBoolean(isLoading);
        this.WriteInteger(data.Id);
        this.WriteString(data.Name);
        this.WriteInteger(data.OwnerId);
        this.WriteString(data.OwnerName);
        this.WriteInteger(session.GetUser().IsTeleporting ? 0 : (int)data.Access);
        this.WriteInteger(data.UsersNow);
        this.WriteInteger(data.UsersMax);
        this.WriteString(data.Description);
        this.WriteInteger(data.TrocStatus);
        this.WriteInteger(data.Score);
        this.WriteInteger(0);//Top rated room rank.
        this.WriteInteger(data.Category);

        this.WriteInteger(data.Tags.Count);
        foreach (var tag in data.Tags)
        {
            this.WriteString(tag);
        }


        if (data.Group != null)
        {
            this.WriteInteger(58);//What?
            this.WriteInteger(data.Group == null ? 0 : data.Group.Id);
            this.WriteString(data.Group == null ? "" : data.Group.Name);
            this.WriteString(data.Group == null ? "" : data.Group.Badge);
        }
        else
        {
            this.WriteInteger(56);//What?
        }


        this.WriteBoolean(checkEntry);
        this.WriteBoolean(false);
        this.WriteBoolean(false);
        this.WriteBoolean(false);

        this.WriteInteger(data.MuteFuse); // who can mute
        this.WriteInteger(data.WhoCanKick); // who can kick
        this.WriteInteger(data.BanFuse); // who can ban

        this.WriteBoolean((session != null) && data.OwnerName.ToLower() != session.GetUser().Username.ToLower());
        this.WriteInteger(data.ChatType);  //ChatMode, ChatSize, ChatSpeed, HearingDistance, ExtraFlood is the order.
        this.WriteInteger(data.ChatBalloon);
        this.WriteInteger(data.ChatSpeed);
        this.WriteInteger(data.ChatMaxDistance);
        this.WriteInteger(data.ChatFloodProtection);
    }
}
