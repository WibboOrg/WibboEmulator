using Butterfly.Game.Clients;
using Butterfly.Game.Rooms.AI;
using System.Drawing;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class SuperBot : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if (Params.Length > 1)
            {
                int count;
                if (Params.Length == 2)
                {
                    int.TryParse(Params[1], out count);
                    for (int i = 0; i < count; i++)
                    {
                        if (!Room.IsRoleplay)
                        {
                            RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, Session.GetUser().Id, Room.Id, BotAIType.SuperBot, false, Session.GetUser().Username, "SuperBot", Session.GetUser().Gender, Session.GetUser().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                            superBot.BotData.FollowUser = UserRoom.VirtualId;
                        }
                        else
                        {
                            Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, Session.GetUser().Id, Room.Id, BotAIType.SuperBot, false, Session.GetUser().Username, "SuperBot", Session.GetUser().Gender, Session.GetUser().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        }
                    }
                }
                else if (Params.Length > 2)
                {
                    RoomUser GetUserRoom = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByName(Params[1]);
                    if (GetUserRoom == null)
                    {
                        return;
                    }

                    if (Session.Langue != GetUserRoom.GetClient().Langue)
                    {
                        Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", GetUserRoom.GetClient().Langue), Session.Langue));
                        return;
                    }

                    int.TryParse(Params[2], out count);
                    for (int i = 0; i < count; i++)
                    {
                        RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(-i, GetUserRoom.GetClient().GetUser().Id, Room.Id, BotAIType.SuperBot, false, GetUserRoom.GetClient().GetUser().Username, "SuperBot", GetUserRoom.GetClient().GetUser().Gender, GetUserRoom.GetClient().GetUser().Look, GetUserRoom.X, GetUserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                        superBot.BotData.FollowUser = GetUserRoom.VirtualId;
                    }
                }
            }
            else
            {
                ICollection<Client> Users = ButterflyEnvironment.GetGame().GetClientManager().GetClients;

                if (Users == null)
                {
                    return;
                }

                foreach (Client GameClient in Users)
                {
                    if (GameClient.GetUser() == null)
                    {
                        continue;
                    }

                    Point randomWalkableSquare = Room.GetGameMap().getRandomWalkableSquare(UserRoom.X, UserRoom.Y);

                    RoomUser superBot = Room.GetRoomUserManager().DeploySuperBot(new RoomBot(0, Session.GetUser().Id, Room.Id, BotAIType.SuperBot, false, GameClient.GetUser().Username, GameClient.GetUser().Motto, GameClient.GetUser().Gender, GameClient.GetUser().Look, UserRoom.X, UserRoom.Y, 0, 2, false, "", 0, false, 0, 0, 0));
                    superBot.MoveTo(randomWalkableSquare, true);
                }
            }
        }
    }
}
