using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class DeleteMission : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string username = Params[1];
            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (TargetUser == null)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetHabbo().Rank <= TargetUser.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.notpermitted", Session.Langue));
            }
            else
            {
                TargetUser.GetHabbo().Motto = ButterflyEnvironment.GetLanguageManager().TryGetValue("user.unacceptable_motto", TargetUser.Langue);
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserDao.UpdateMotto(dbClient, TargetUser.GetHabbo().Id, TargetUser.GetHabbo().Motto);
                }

                Room currentRoom2 = TargetUser.GetHabbo().CurrentRoom;
                if (currentRoom2 == null)
                {
                    return;
                }

                RoomUser roomUserByHabbo = currentRoom2.GetRoomUserManager().GetRoomUserByHabboId(TargetUser.GetHabbo().Id);
                if (roomUserByHabbo == null)
                {
                    return;
                }

                currentRoom2.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
            }

        }
    }
}
