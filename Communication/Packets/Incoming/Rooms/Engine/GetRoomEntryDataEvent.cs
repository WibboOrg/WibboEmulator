namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

internal sealed class GetRoomEntryDataEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null)
        {
            return;
        }

        if (Session.User.LoadingRoomId == 0)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.LoadingRoomId, out var room))
        {
            return;
        }

        if (room.RoomData.Access == RoomAccess.Doorbell)
        {
            if (!Session.User.AllowDoorBell)
            {
                return;
            }
            else
            {
                Session.User.AllowDoorBell = false;
            }
        }

        if (!room.RoomUserManager.AddAvatarToRoom(Session))
        {
            room.RoomUserManager.RemoveUserFromRoom(Session, false, false);
            return;
        }

        room.SendObjects(Session);

        Session.SendPacket(new RoomEntryInfoComposer(room.Id, room.CheckRights(Session, true)));
        Session.SendPacket(new RoomVisualizationSettingsComposer(room.RoomData.WallThickness, room.RoomData.FloorThickness, room.RoomData.HideWall));

        var thisUser = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);

        if (thisUser != null)
        {
            room.SendPacket(new UserChangeComposer(thisUser, false));

            if (!thisUser.IsSpectator)
            {
                room.RoomUserManager.UserEnter(thisUser);
            }
        }

        if (Session.User.Nuxenable)
        {
            Session.SendPacket(new NuxAlertComposer(2));

            Session.User.PassedNuxCount++;
            Session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/hide"));
            Session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/nux.bot.info.navigator.1"));
        }

        if (Session.User.SpamEnable)
        {
            var timeSpan = DateTime.Now - Session.User.SpamFloodTime;
            if (timeSpan.TotalSeconds < Session.User.SpamProtectionTime)
            {
                Session.SendPacket(new FloodControlComposer(Session.User.SpamProtectionTime - timeSpan.Seconds));
            }
        }

        if (room.RoomData.OwnerId != Session.User.Id)
        {
            _ = AchievementManager.ProgressAchievement(Session, "ACH_RoomEntry", 1);
        }

        var timeStampNow = UnixTimestamp.GetNow();

        if (!Session.User.Visits.ContainsKey(timeStampNow))
        {
            Session.User.Visits.Add(timeStampNow, room.Id);
        }
    }
}
