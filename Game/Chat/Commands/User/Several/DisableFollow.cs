using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class DisableFollow : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser().HideInRoom)
            {
                Session.GetUser().HideInRoom = false;
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.followme.true", Session.Langue));
            }
            else
            {
                Session.GetUser().HideInRoom = true;
                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.followme.false", Session.Langue));
            }

        }
    }
}
