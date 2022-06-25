using WibboEmulator.Communication.Packets.Outgoing.Misc;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;
using WibboEmulator.Utilities;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetRoomEntryDataEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null)
            {
                return;
            }

            if (Session.GetUser().LoadingRoomId == 0)
            {
                return;
            }

            Room Room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().LoadingRoomId);
            if (Room == null)
            {
                return;
            }

            if (Room.RoomData.State == 1)
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

            if (!Room.GetRoomUserManager().AddAvatarToRoom(Session))
            {
                Room.GetRoomUserManager().RemoveUserFromRoom(Session, false, false);
                return;
            }

            Room.SendObjects(Session);

            Session.SendPacket(new RoomEntryInfoComposer(Room.Id, Room.CheckRights(Session, true)));
            Session.SendPacket(new RoomVisualizationSettingsComposer(Room.RoomData.WallThickness, Room.RoomData.FloorThickness, Room.RoomData.Hidewall));

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);

            if (ThisUser != null)
            {
                Room.SendPacket(new UserChangeComposer(ThisUser, false));
            }

            if (!ThisUser.IsSpectator)
            {
                Room.GetRoomUserManager().UserEnter(ThisUser);
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

            if (Room.RoomData.OwnerId != Session.GetUser().Id)
            {
                WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomEntry", 1);
            }

            double timeStampNow = UnixTimestamp.GetNow();

            if (!Session.GetUser().Visits.ContainsKey(timeStampNow))
                Session.GetUser().Visits.Add(timeStampNow, Room.Id);
        }
    }
}
