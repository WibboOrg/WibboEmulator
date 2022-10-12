namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using System.Text;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

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

        var name = packet.PopString();
        var description = packet.PopString();
        var state = packet.PopInt();
        var password = packet.PopString();
        var maxUsers = packet.PopInt();
        var categoryId = packet.PopInt();
        var tagCount = packet.PopInt();
        var tags = new List<string>();
        var stringBuilder = new StringBuilder();
        for (var index = 0; index < tagCount; ++index)
        {
            if (index > 0)
            {
                _ = stringBuilder.Append(',');
            }

            var tag = packet.PopString().ToLower();
            tags.Add(tag);
            _ = stringBuilder.Append(tag);
        }
        var trocStatus = packet.PopInt();
        var allowPets = packet.PopBoolean();
        var allowPetsEat = packet.PopBoolean();
        var allowWalkthrough = packet.PopBoolean();
        var hideWall = packet.PopBoolean();
        var wallThickness = packet.PopInt();
        var floorThickness = packet.PopInt();
        var mutefuse = packet.PopInt();
        var kickfuse = packet.PopInt();
        var banfuse = packet.PopInt();
        var chatType = packet.PopInt();
        var chatBalloon = packet.PopInt();
        var chatSpeed = packet.PopInt();
        var chatMaxDistance = packet.PopInt();
        var chatFloodProtection = packet.PopInt();

        if (wallThickness is < (-2) or > 1)
        {
            wallThickness = 0;
        }

        if (floorThickness is < (-2) or > 1)
        {
            floorThickness = 0;
        }

        if (name.Length is < 1 or > 100)
        {
            return;
        }

        if (state is < 0 or > 3)
        {
            return;
        }

        if (maxUsers is < 10 or > 75)
        {
            maxUsers = 25;
        }

        if (trocStatus is < 0 or > 2)
        {
            trocStatus = 0;
        }

        if (tagCount > 2 || (mutefuse != 0 && mutefuse != 1) || (kickfuse != 0 && kickfuse != 1 && kickfuse != 2) || (banfuse != 0 && banfuse != 1))
        {
            return;
        }

        if (chatMaxDistance > 99)
        {
            chatMaxDistance = 99;
        }

        room.Data.AllowPets = allowPets;
        room.Data.AllowPetsEating = allowPetsEat;
        room.Data.AllowWalkthrough = allowWalkthrough;
        room.Data.HideWall = hideWall;
        room.Data.Name = name;
        room.Data.Access = state.ToEnum(RoomAccess.Open);
        room.Data.Description = description;
        room.Data.Category = categoryId;
        if (!string.IsNullOrEmpty(password))
        {
            room.Data.Password = password;
        }

        room.ClearTags();
        room.AddTagRange(tags);
        room.Data.Tags.Clear();
        room.Data.Tags.AddRange(tags);
        room.Data.UsersMax = maxUsers;
        room.Data.WallThickness = wallThickness;
        room.Data.FloorThickness = floorThickness;
        room.Data.MuteFuse = mutefuse;
        room.Data.WhoCanKick = kickfuse;
        room.Data.BanFuse = banfuse;

        room.Data.ChatType = chatType;
        room.Data.ChatBalloon = chatBalloon;
        room.Data.ChatSpeed = chatSpeed;
        room.Data.ChatMaxDistance = chatMaxDistance;
        room.Data.ChatFloodProtection = chatFloodProtection;

        room.Data.TrocStatus = trocStatus;

        var accessState = "open";
        if (room.Data.Access == RoomAccess.Doorbell)
        {
            accessState = "locked";
        }
        else if (room.Data.Access == RoomAccess.Password)
        {
            accessState = "password";
        }
        else if (room.Data.Access == RoomAccess.Invisible)
        {
            accessState = "hide";
        }

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            RoomDao.UpdateAll(dbClient, room.Id, room.Data.Name, room.Data.Description, room.Data.Password, stringBuilder.ToString(), categoryId, accessState, maxUsers, allowPets, allowPetsEat, allowWalkthrough, room.Data.HideWall, room.Data.FloorThickness, room.Data.WallThickness, mutefuse, kickfuse, banfuse, chatType, chatBalloon, chatSpeed, chatMaxDistance, chatFloodProtection, trocStatus);
        }

        session.SendPacket(new RoomSettingsSavedComposer(room.Id));

        room.SendPacket(new RoomVisualizationSettingsComposer(room.Data.WallThickness, room.Data.FloorThickness, room.Data.HideWall));
        room.SendPacket(new RoomChatOptionsComposer(room.Data.ChatType, room.Data.ChatBalloon, room.Data.ChatSpeed, room.Data.ChatMaxDistance, room.Data.ChatFloodProtection));

        session.SendPacket(new GetGuestRoomResultComposer(session, room.Data, true, false));
    }
}
