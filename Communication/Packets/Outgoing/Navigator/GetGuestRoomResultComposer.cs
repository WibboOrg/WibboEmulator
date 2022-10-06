namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class GetGuestRoomResultComposer : ServerPacket
{
    public GetGuestRoomResultComposer(GameClient session, RoomData Data, bool isLoading, bool checkEntry)
        : base(ServerPacketHeader.ROOM_INFO)
    {
        this.WriteBoolean(isLoading);
        this.WriteInteger(Data.Id);
        this.WriteString(Data.Name);
        this.WriteInteger(Data.OwnerId);
        this.WriteString(Data.OwnerName);
        this.WriteInteger(session.GetUser().IsTeleporting ? 0 : Data.State);
        this.WriteInteger(Data.UsersNow);
        this.WriteInteger(Data.UsersMax);
        this.WriteString(Data.Description);
        this.WriteInteger(Data.TrocStatus);
        this.WriteInteger(Data.Score);
        this.WriteInteger(0);//Top rated room rank.
        this.WriteInteger(Data.Category);

        this.WriteInteger(Data.Tags.Count);
        foreach (var Tag in Data.Tags)
        {
            this.WriteString(Tag);
        }


        if (Data.Group != null)
        {
            this.WriteInteger(58);//What?
            this.WriteInteger(Data.Group == null ? 0 : Data.Group.Id);
            this.WriteString(Data.Group == null ? "" : Data.Group.Name);
            this.WriteString(Data.Group == null ? "" : Data.Group.Badge);
        }
        else
        {
            this.WriteInteger(56);//What?
        }


        this.WriteBoolean(checkEntry);
        this.WriteBoolean(false);
        this.WriteBoolean(false);
        this.WriteBoolean(false);

        this.WriteInteger(Data.MuteFuse); // who can mute
        this.WriteInteger(Data.WhoCanKick); // who can kick
        this.WriteInteger(Data.BanFuse); // who can ban

        this.WriteBoolean((session != null) && Data.OwnerName.ToLower() != session.GetUser().Username.ToLower());
        this.WriteInteger(Data.ChatType);  //ChatMode, ChatSize, ChatSpeed, HearingDistance, ExtraFlood is the order.
        this.WriteInteger(Data.ChatBalloon);
        this.WriteInteger(Data.ChatSpeed);
        this.WriteInteger(Data.ChatMaxDistance);
        this.WriteInteger(Data.ChatFloodProtection);
    }
}
