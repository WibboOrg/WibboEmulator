using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class DisableFriendRequests : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().HasFriendRequestsDisabled)
            {
                Session.GetHabbo().HasFriendRequestsDisabled = false;
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.textamigo.true", Session.Langue));
            }
            else
            {
                Session.GetHabbo().HasFriendRequestsDisabled = true;
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.textamigo.false", Session.Langue));
            }

        }
    }
}
