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
            else if (Session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.notpermitted", Session.Langue));
            }
            else
            {
                TargetUser.GetUser().Motto = ButterflyEnvironment.GetLanguageManager().TryGetValue("user.unacceptable_motto", TargetUser.Langue);
                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    UserDao.UpdateMotto(dbClient, TargetUser.GetUser().Id, TargetUser.GetUser().Motto);
                }

                Room currentRoom2 = TargetUser.GetUser().CurrentRoom;
                if (currentRoom2 == null)
                {
                    return;
                }

                RoomUser roomUserByUserId = currentRoom2.GetRoomUserManager().GetRoomUserByUserId(TargetUser.GetUser().Id);
                if (roomUserByUserId == null)
                {
                    return;
                }

                currentRoom2.SendPacket(new UserChangeComposer(roomUserByUserId, false));
            }

        }
    }
}
