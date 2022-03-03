using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Mimic : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            //if (UserRoom.team != Team.none || UserRoom.InGame)
            //return;

            if (Room.IsRoleplay && !Room.CheckRights(Session))
            {
                return;
            }

            if (Params.Length != 2)
            {
                return;
            }

            string Username = Params[1];

            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Username);
                if (Bot == null || Bot.BotData == null)
                {
                    return;
                }

                Session.GetUser().Gender = Bot.BotData.Gender;
                Session.GetUser().Look = Bot.BotData.Look;
            }
            else
            {

                if (TargetUser.GetUser().PremiumProtect && !Session.GetUser().HasFuse("fuse_mod"))
                {
                    UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                    return;
                }

                Session.GetUser().Gender = TargetUser.GetUser().Gender;
                Session.GetUser().Look = TargetUser.GetUser().Look;
            }

            if (UserRoom.transformation || UserRoom.IsSpectator)
            {
                return;
            }

            if (!Session.GetUser().InRoom)
            {
                return;
            }

            Room currentRoom = Session.GetUser().CurrentRoom;
            if (currentRoom == null)
            {
                return;
            }

            RoomUser roomUserByUserId = UserRoom;
            if (roomUserByUserId == null)
            {
                return;
            }

            Session.SendPacket(new UserChangeComposer(roomUserByUserId, true));
            currentRoom.SendPacket(new UserChangeComposer(roomUserByUserId, false));

        }
    }
}
