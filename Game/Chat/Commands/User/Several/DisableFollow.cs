using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class DisableFollow : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser().HideInRoom)
            {
                Session.GetUser().HideInRoom = false;
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.followme.true", Session.Langue));
            }
            else
            {
                Session.GetUser().HideInRoom = true;
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.followme.false", Session.Langue));
            }
        }
    }
}
