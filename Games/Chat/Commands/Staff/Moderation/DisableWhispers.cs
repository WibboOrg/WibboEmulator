using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class DisableWhispers : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetUser().ViewMurmur)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.true", Session.Langue));
            }
            else
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.false", Session.Langue));
            }

            Session.GetUser().ViewMurmur = !Session.GetUser().ViewMurmur;
        }
    }
}