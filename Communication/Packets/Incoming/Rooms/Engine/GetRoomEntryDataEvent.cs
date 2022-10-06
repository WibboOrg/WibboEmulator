namespace WibboEmulator.Communication.Packets.Incoming.Rooms.Engine;
using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Utilities;

internal class GetRoomEntryDataEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null)
        {
            return;
        }

        if (session.GetUser().LoadingRoomId == 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().LoadingRoomId, out var room))
        {
            return;
        }

        if (room.RoomData.State == 1)
        {
            if (!session.GetUser().AllowDoorBell)
            {
                return;
            }
            else
            {
                session.GetUser().AllowDoorBell = false;
            }
        }

        if (!room.GetRoomUserManager().AddAvatarToRoom(session))
        {
            room.GetRoomUserManager().RemoveUserFromRoom(session, false, false);
            return;
        }

        room.SendObjects(session);

        session.SendPacket(new RoomEntryInfoComposer(room.Id, room.CheckRights(session, true)));
        session.SendPacket(new RoomVisualizationSettingsComposer(room.RoomData.WallThickness, room.RoomData.FloorThickness, room.RoomData.Hidewall));

        var ThisUser = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);

        if (ThisUser != null)
        {
            room.SendPacket(new UserChangeComposer(ThisUser, false));

            if (!ThisUser.IsSpectator)
            {
                room.GetRoomUserManager().UserEnter(ThisUser);
            }
        }

        if (session.GetUser().Nuxenable)
        {
            session.SendPacket(new NuxAlertComposer(2));

            session.GetUser().PassedNuxCount++;
            session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/hide"));
            session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/nux.bot.info.navigator.1"));
        }

        if (session.GetUser().SpamEnable)
        {
            var timeSpan = DateTime.Now - session.GetUser().SpamFloodTime;
            if (timeSpan.TotalSeconds < session.GetUser().SpamProtectionTime)
            {
                session.SendPacket(new FloodControlComposer(session.GetUser().SpamProtectionTime - timeSpan.Seconds));
            }
        }

        if (room.RoomData.OwnerId != session.GetUser().Id)
        {
            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RoomEntry", 1);
        }

        var timeStampNow = UnixTimestamp.GetNow();

        if (!session.GetUser().Visits.ContainsKey(timeStampNow))
        {
            session.GetUser().Visits.Add(timeStampNow, room.Id);
        }
    }
}
