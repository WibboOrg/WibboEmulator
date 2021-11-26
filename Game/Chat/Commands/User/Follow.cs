using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Follow : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            Client clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
            }
            else if ((clientByUsername.GetHabbo().HideInRoom) && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.follow.notallowed", Session.Langue));
            }
            else
            {
                Room currentRoom = clientByUsername.GetHabbo().CurrentRoom;
                if (currentRoom != null)
                {
                    Session.SendPacket(new GetGuestRoomResultComposer(Session, currentRoom.RoomData, false, true));
                }
            }

        }
    }
}
