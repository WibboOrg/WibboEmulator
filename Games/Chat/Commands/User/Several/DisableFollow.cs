using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class DisableFollow : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
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
