using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Groups;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class DeleteGroup : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            if (Room != null)
            {
                Room.RoomData.Group = null;
            }

            //using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
               // dbClient.RunQuery("DELETE FROM `groups` WHERE `id` = '" + Room.Group.Id + "'");
               // dbClient.RunQuery("DELETE FROM `group_memberships` WHERE `group_id` = '" + Room.RoomData.Group.Id + "'");
               // dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Room.RoomData.Group.Id + "'");
               // dbClient.RunQuery("UPDATE `rooms` SET `group_id` = '0' WHERE `group_id` = '" + Room.RoomData.Group.Id + "' LIMIT 1");
               // dbClient.RunQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `groupid` = '" + Room.RoomData.Group.Id + "' LIMIT 1");
               // dbClient.RunQuery("DELETE FROM `items_groups` WHERE `group_id` = '" + Room.RoomData.Group.Id + "'");
            }

            ButterflyEnvironment.GetGame().GetGroupManager().DeleteGroup(Room.RoomData.Group.Id);

            Room.RoomData.Group = null;

            ButterflyEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom);

            Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("notif.groupdelete.succes", Session.Langue));
            return;

        }
    }
}
