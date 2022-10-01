using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class FastWalk : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.WalkSpeed = !UserRoom.WalkSpeed;

            if (UserRoom.WalkSpeed)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.true", Session.Langue));
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.false", Session.Langue));
            }
        }
    }
}
