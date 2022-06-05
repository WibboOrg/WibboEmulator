using Wibbo.Communication.Packets.Outgoing.Navigator;
using Wibbo.Game.Rooms;
using Wibbo.Game.Clients;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Follow : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetUser == null || TargetUser.GetUser() == null)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
            }
            else if ((TargetUser.GetUser().HideInRoom) && !Session.GetUser().HasFuse("fuse_mod"))
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", Session.Langue));
            }
            else
            {
                Room currentRoom = TargetUser.GetUser().CurrentRoom;
                if (currentRoom != null)
                {
                    Session.SendPacket(new GetGuestRoomResultComposer(Session, currentRoom.RoomData, false, true));
                }
            }

        }
    }
}
