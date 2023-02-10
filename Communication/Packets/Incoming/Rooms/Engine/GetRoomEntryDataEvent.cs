namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal sealed class GetRoomEntryDataEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null)
        {
            return;
        }

        if (session.User.LoadingRoomId == 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.LoadingRoomId, out var room))
        {
            return;
        }

        if (room.RoomData.Access == RoomAccess.Doorbell)
        {
            if (!session.User.AllowDoorBell)
            {
                return;
            }
            else
            {
                session.User.AllowDoorBell = false;
            }
        }

        if (!room.RoomUserManager.AddAvatarToRoom(session))
        {
            room.RoomUserManager.RemoveUserFromRoom(session, false, false);
            return;
        }

        room.SendObjects(session);

        session.SendPacket(new RoomEntryInfoComposer(room.Id, room.CheckRights(session, true)));
        session.SendPacket(new RoomVisualizationSettingsComposer(room.RoomData.WallThickness, room.RoomData.FloorThickness, room.RoomData.HideWall));

        var thisUser = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);

        if (thisUser != null)
        {
            room.SendPacket(new UserChangeComposer(thisUser, false));

            if (!thisUser.IsSpectator)
            {
                room.RoomUserManager.UserEnter(thisUser);
            }
        }

        if (session.User.Nuxenable)
        {
            session.SendPacket(new NuxAlertComposer(2));

            session.User.PassedNuxCount++;
            session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/hide"));
            session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/nux.bot.info.navigator.1"));
        }

        if (session.User.SpamEnable)
        {
            var timeSpan = DateTime.Now - session.User.SpamFloodTime;
            if (timeSpan.TotalSeconds < session.User.SpamProtectionTime)
            {
                session.SendPacket(new FloodControlComposer(session.User.SpamProtectionTime - timeSpan.Seconds));
            }
        }

        if (room.RoomData.OwnerId != session.User.Id)
        {
            _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RoomEntry", 1);
        }

        var timeStampNow = UnixTimestamp.GetNow();

        if (!session.User.Visits.ContainsKey(timeStampNow))
        {
            session.User.Visits.Add(timeStampNow, room.Id);
        }
    }
}
