namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Settings;
using System.Text;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Room;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal sealed class SaveRoomSettingsEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var roomId = packet.PopInt();

        if (!RoomManager.TryGetRoom(roomId, out var room))
        {
            return;
        }

        if (!room.CheckRights(session, true) && !session.User.HasPermission("settings_room"))
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

        if (maxUsers is < 10 or > 100)
        {
            maxUsers = 100;
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

        room.RoomData.AllowPets = allowPets;
        room.RoomData.AllowPetsEating = allowPetsEat;
        room.RoomData.AllowWalkthrough = allowWalkthrough;
        room.RoomData.HideWall = hideWall;
        room.RoomData.Name = name;
        room.RoomData.Access = state.ToEnum(RoomAccess.Open);
        room.RoomData.Description = description;
        room.RoomData.Category = categoryId;
        if (!string.IsNullOrEmpty(password))
        {
            room.RoomData.Password = password;
        }

        room.ClearTags();
        room.AddTagRange(tags);
        room.RoomData.Tags.Clear();
        room.RoomData.Tags.AddRange(tags);
        room.RoomData.UsersMax = maxUsers;
        room.RoomData.WallThickness = wallThickness;
        room.RoomData.FloorThickness = floorThickness;
        room.RoomData.MuteFuse = mutefuse;
        room.RoomData.WhoCanKick = kickfuse;
        room.RoomData.BanFuse = banfuse;

        room.RoomData.ChatType = chatType;
        room.RoomData.ChatBalloon = chatBalloon;
        room.RoomData.ChatSpeed = chatSpeed;
        room.RoomData.ChatMaxDistance = chatMaxDistance;
        room.RoomData.ChatFloodProtection = chatFloodProtection;

        room.RoomData.TrocStatus = trocStatus;

        var accessState = "open";
        if (room.RoomData.Access == RoomAccess.Doorbell)
        {
            accessState = "locked";
        }
        else if (room.RoomData.Access == RoomAccess.Password)
        {
            accessState = "password";
        }
        else if (room.RoomData.Access == RoomAccess.Hide)
        {
            accessState = "hide";
        }

        using (var dbClient = DatabaseManager.Connection)
        {
            RoomDao.UpdateAll(dbClient, room.Id, room.RoomData.Name, room.RoomData.Description, room.RoomData.Password, stringBuilder.ToString(), categoryId, accessState, maxUsers, allowPets, allowPetsEat, allowWalkthrough, room.RoomData.HideWall, room.RoomData.FloorThickness, room.RoomData.WallThickness, mutefuse, kickfuse, banfuse, chatType, chatBalloon, chatSpeed, chatMaxDistance, chatFloodProtection, trocStatus);
        }

        session.SendPacket(new RoomSettingsSavedComposer(room.Id));

        room.SendPacket(new RoomVisualizationSettingsComposer(room.RoomData.WallThickness, room.RoomData.FloorThickness, room.RoomData.HideWall));
        room.SendPacket(new RoomChatOptionsComposer(room.RoomData.ChatType, room.RoomData.ChatBalloon, room.RoomData.ChatSpeed, room.RoomData.ChatMaxDistance, room.RoomData.ChatFloodProtection));

        session.SendPacket(new GetGuestRoomResultComposer(session, room.RoomData, true, false));
    }
}
