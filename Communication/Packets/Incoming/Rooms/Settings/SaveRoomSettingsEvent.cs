namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using System.Text;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;

internal class SaveRoomSettingsEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var roomId = packet.PopInt();

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true) && !session.GetUser().HasPermission("perm_settings_room"))
        {
            return;
        }

        var Name = packet.PopString();
        var Description = packet.PopString();
        var State = packet.PopInt();
        var Password = packet.PopString();
        var MaxUsers = packet.PopInt();
        var CategoryId = packet.PopInt();
        var TagCount = packet.PopInt();
        var tags = new List<string>();
        var stringBuilder = new StringBuilder();
        for (var index = 0; index < TagCount; ++index)
        {
            if (index > 0)
            {
                _ = stringBuilder.Append(',');
            }

            var tag = packet.PopString().ToLower();
            tags.Add(tag);
            _ = stringBuilder.Append(tag);
        }
        var TrocStatus = packet.PopInt();
        var AllowPets = packet.PopBoolean();
        var AllowPetsEat = packet.PopBoolean();
        var AllowWalkthrough = packet.PopBoolean();
        var Hidewall = packet.PopBoolean();
        var WallThickness = packet.PopInt();
        var FloorThickness = packet.PopInt();
        var mutefuse = packet.PopInt();
        var kickfuse = packet.PopInt();
        var banfuse = packet.PopInt();
        var ChatType = packet.PopInt();
        var ChatBalloon = packet.PopInt();
        var ChatSpeed = packet.PopInt();
        var ChatMaxDistance = packet.PopInt();
        var ChatFloodProtection = packet.PopInt();

        if (WallThickness is < (-2) or > 1)
        {
            WallThickness = 0;
        }

        if (FloorThickness is < (-2) or > 1)
        {
            FloorThickness = 0;
        }

        if (Name.Length is < 1 or > 100)
        {
            return;
        }

        if (State is < 0 or > 3)
        {
            return;
        }

        if (MaxUsers is < 10 or > 75)
        {
            MaxUsers = 25;
        }

        if (TrocStatus is < 0 or > 2)
        {
            TrocStatus = 0;
        }

        if (TagCount > 2 || (mutefuse != 0 && mutefuse != 1) || (kickfuse != 0 && kickfuse != 1 && kickfuse != 2) || (banfuse != 0 && banfuse != 1))
        {
            return;
        }

        if (ChatMaxDistance > 99)
        {
            ChatMaxDistance = 99;
        }

        room.RoomData.AllowPets = AllowPets;
        room.RoomData.AllowPetsEating = AllowPetsEat;
        room.RoomData.AllowWalkthrough = AllowWalkthrough;
        room.RoomData.Hidewall = Hidewall;
        room.RoomData.Name = Name;
        room.RoomData.State = State;
        room.RoomData.Description = Description;
        room.RoomData.Category = CategoryId;
        if (!string.IsNullOrEmpty(Password))
        {
            room.RoomData.Password = Password;
        }

        room.ClearTags();
        room.AddTagRange(tags);
        room.RoomData.Tags.Clear();
        room.RoomData.Tags.AddRange(tags);
        room.RoomData.UsersMax = MaxUsers;
        room.RoomData.WallThickness = WallThickness;
        room.RoomData.FloorThickness = FloorThickness;
        room.RoomData.MuteFuse = mutefuse;
        room.RoomData.WhoCanKick = kickfuse;
        room.RoomData.BanFuse = banfuse;

        room.RoomData.ChatType = ChatType;
        room.RoomData.ChatBalloon = ChatBalloon;
        room.RoomData.ChatSpeed = ChatSpeed;
        room.RoomData.ChatMaxDistance = ChatMaxDistance;
        room.RoomData.ChatFloodProtection = ChatFloodProtection;

        room.RoomData.TrocStatus = TrocStatus;
        var str5 = "open";
        if (room.RoomData.State == 1)
        {
            str5 = "locked";
        }
        else if (room.RoomData.State == 2)
        {
            str5 = "password";
        }
        else if (room.RoomData.State == 3)
        {
            str5 = "hide";
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateAll(dbClient, room.Id, room.RoomData.Name, room.RoomData.Description, room.RoomData.Password, stringBuilder.ToString(), CategoryId, str5, MaxUsers, AllowPets, AllowPetsEat, AllowWalkthrough, room.RoomData.Hidewall, room.RoomData.FloorThickness, room.RoomData.WallThickness, mutefuse, kickfuse, banfuse, ChatType, ChatBalloon, ChatSpeed, ChatMaxDistance, ChatFloodProtection, TrocStatus);
        }

        session.SendPacket(new RoomSettingsSavedComposer(room.Id));

        room.SendPacket(new RoomVisualizationSettingsComposer(room.RoomData.WallThickness, room.RoomData.FloorThickness, room.RoomData.Hidewall));
        room.SendPacket(new RoomChatOptionsComposer(room.RoomData.ChatType, room.RoomData.ChatBalloon, room.RoomData.ChatSpeed, room.RoomData.ChatMaxDistance, room.RoomData.ChatFloodProtection));

        session.SendPacket(new GetGuestRoomResultComposer(session, room.RoomData, true, false));
    }
}
