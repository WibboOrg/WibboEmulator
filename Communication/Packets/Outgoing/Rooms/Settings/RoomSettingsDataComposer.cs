namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.Rooms;

internal class RoomSettingsDataComposer : ServerPacket
{
    public RoomSettingsDataComposer(RoomData Room)
        : base(ServerPacketHeader.ROOM_SETTINGS)
    {
        this.WriteInteger(Room.Id);
        this.WriteString(Room.Name);
        this.WriteString(Room.Description);
        this.WriteInteger(Room.State);
        this.WriteInteger(Room.Category);
        this.WriteInteger(Room.UsersMax);
        this.WriteInteger(((Room.Model.MapSizeX * Room.Model.MapSizeY) > 100) ? 50 : 25);

        this.WriteInteger(Room.Tags.Count);
        foreach (var Tag in Room.Tags.ToArray())
        {
            this.WriteString(Tag);
        }

        this.WriteInteger(Room.TrocStatus); //Trade
        this.WriteInteger(Room.AllowPets ? 1 : 0); // allows pets in room - pet system lacking, so always off
        this.WriteInteger(Room.AllowPetsEating ? 1 : 0);// allows pets to eat your food - pet system lacking, so always off
        this.WriteInteger(Room.AllowWalkthrough ? 1 : 0);
        this.WriteInteger(Room.Hidewall ? 1 : 0);
        this.WriteInteger(Room.WallThickness);
        this.WriteInteger(Room.FloorThickness);

        this.WriteInteger(Room.ChatType);//Chat mode
        this.WriteInteger(Room.ChatBalloon);//Chat size
        this.WriteInteger(Room.ChatSpeed);//Chat speed
        this.WriteInteger(Room.ChatMaxDistance);//Hearing Distance
        this.WriteInteger(Room.ChatFloodProtection);//Additional Flood

        this.WriteBoolean(true);

        this.WriteInteger(Room.MuteFuse); // who can mute
        this.WriteInteger(Room.WhoCanKick); // who can kick
        this.WriteInteger(Room.BanFuse); // who can ban
    }
}
