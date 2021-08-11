using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;
using System;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GetRoomEntryDataEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            if (Session.GetHabbo().LoadingRoomId == 0)
            {
                return;
            }

            Room Room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().LoadingRoomId);
            if (Room == null)
            {
                return;
            }

            if (Room.RoomData.State == 1)
            {
                if (!Session.GetHabbo().AllowDoorBell)
                {
                    return;
                }
                else
                {
                    Session.GetHabbo().AllowDoorBell = false;
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

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (ThisUser != null)
            {
                Room.SendPacket(new UserChangeComposer(ThisUser, false));
            }

            if (!ThisUser.IsSpectator)
            {
                Room.GetRoomUserManager().UserEnter(ThisUser);
            }

            if (Session.GetHabbo().Nuxenable)
            {
                ServerPacket nuxStatus = new ServerPacket(ServerPacketHeader.NuxAlertComposer);
                nuxStatus.WriteInteger(2);
                Session.SendPacket(nuxStatus);

                Session.GetHabbo().PassedNuxCount++;
                Session.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
                Session.SendPacket(new NuxAlertComposer("helpBubble/add/BOTTOM_BAR_NAVIGATOR/nux.bot.info.navigator.1"));
            }

            if (Session.GetHabbo().spamEnable)
            {
                TimeSpan timeSpan = DateTime.Now - Session.GetHabbo().spamFloodTime;
                if (timeSpan.TotalSeconds < Session.GetHabbo().spamProtectionTime)
                {
                    Session.SendPacket(new FloodControlComposer(Session.GetHabbo().spamProtectionTime - timeSpan.Seconds));
                }
            }

            if (Room.RoomData.OwnerId != Session.GetHabbo().Id)
            {
                ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RoomEntry", 1);
            }
        }
    }
}
