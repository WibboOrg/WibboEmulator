using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetRoomEntryDataEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (Session.GetUser().LoadingRoomId == 0)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().LoadingRoomId, out Room room))
                return;

            if (room.RoomData.State == 1)
            {
                if (!Session.GetUser().AllowDoorBell)
                {
                    return;
                }
                else
                {
                    Session.GetUser().AllowDoorBell = false;
                }
            }

            if (!room.GetRoomUserManager().AddAvatarToRoom(Session))
            {
                room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                return;
            }

            room.SendObjects(Session);

            Session.SendPacket(new RoomEntryInfoComposer(room.Id, room.CheckRights(Session, true)));
            Session.SendPacket(new RoomVisualizationSettingsComposer(room.RoomData.WallThickness, room.RoomData.FloorThickness, room.RoomData.Hidewall));

            RoomUser ThisUser = room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

            if (ThisUser != null)
            {
                room.SendPacket(new UserChangeComposer(ThisUser, false));

                if (!ThisUser.IsSpectator)
                {
                    room.GetRoomUserManager().UserEnter(ThisUser);
                }
            }

            if (Session.GetUser().Nuxenable)
            {
                Session.SendPacket(new NuxAlertComposer(2));

                Session.GetUser().PassedNuxCount++;
                Session.SendPacket(new InClientLinkComposer("nux/lobbyoffer/hide"));
                Session.SendPacket(new InClientLinkComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/nux.bot.info.navigator.1"));
            }

            if (Session.GetUser().SpamEnable)
            {
                TimeSpan timeSpan = DateTime.Now - Session.GetUser().SpamFloodTime;
                if (timeSpan.TotalSeconds < Session.GetUser().SpamProtectionTime)
                {
                    Session.SendPacket(new FloodControlComposer(Session.GetUser().SpamProtectionTime - timeSpan.Seconds));
                }
            }

            if (room.RoomData.OwnerId != Session.GetUser().Id)
            {
                WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomEntry", 1);
            }

            double timeStampNow = UnixTimestamp.GetNow();

            if (!Session.GetUser().Visits.ContainsKey(timeStampNow))
                Session.GetUser().Visits.Add(timeStampNow, room.Id);
        }
    }
}
