using Wibbo.Communication.Packets.Outgoing.Rooms.Engine;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
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
            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (TargetUser == null)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetUser().Rank <= TargetUser.GetUser().Rank)
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.notpermitted", Session.Langue));
            }
            else
            {
                TargetUser.GetUser().Motto = WibboEnvironment.GetLanguageManager().TryGetValue("user.unacceptable_motto", TargetUser.Langue);
                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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
