namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Games.Rooms;

internal class RoomSettingsDataComposer : ServerPacket
{
    public RoomSettingsDataComposer(RoomData room)
        : base(ServerPacketHeader.ROOM_SETTINGS)
    {
        this.WriteInteger(room.Id);
        this.WriteString(room.Name);
        this.WriteString(room.Description);
        this.WriteInteger(room.State);
        this.WriteInteger(room.Category);
        this.WriteInteger(room.UsersMax);
        this.WriteInteger(((room.Model.MapSizeX * room.Model.MapSizeY) > 100) ? 50 : 25);

        this.WriteInteger(room.Tags.Count);
        foreach (var tag in room.Tags.ToArray())
        {
            this.WriteString(tag);
        }

        this.WriteInteger(room.TrocStatus); //Trade
        this.WriteInteger(room.AllowPets ? 1 : 0); // allows pets in room - pet system lacking, so always off
        this.WriteInteger(room.AllowPetsEating ? 1 : 0);// allows pets to eat your food - pet system lacking, so always off
        this.WriteInteger(room.AllowWalkthrough ? 1 : 0);
        this.WriteInteger(room.Hidewall ? 1 : 0);
        this.WriteInteger(room.WallThickness);
        this.WriteInteger(room.FloorThickness);

        this.WriteInteger(room.ChatType);//Chat mode
        this.WriteInteger(room.ChatBalloon);//Chat size
        this.WriteInteger(room.ChatSpeed);//Chat speed
        this.WriteInteger(room.ChatMaxDistance);//Hearing Distance
        this.WriteInteger(room.ChatFloodProtection);//Additional Flood

        this.WriteBoolean(true);

        this.WriteInteger(room.MuteFuse); // who can mute
        this.WriteInteger(room.WhoCanKick); // who can kick
        this.WriteInteger(room.BanFuse); // who can ban
    }
}
