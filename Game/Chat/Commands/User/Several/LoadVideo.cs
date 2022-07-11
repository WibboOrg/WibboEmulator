using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class LoadVideo : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                UserRoom.LoaderVideoId = "";
                return;
            }
            string Url = Params[1];
            if (string.IsNullOrEmpty(Url) || (!Url.Contains("?v=") && !Url.Contains("youtu.be/"))) //https://youtu.be/_mNig3ZxYbM
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.loadvideo.incorrect", Session.Langue));
                UserRoom.LoaderVideoId = "";
                return;
            }

            string Split = "";

            if (Url.Contains("?v="))
            {
                Split = Url.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];
            }
            else if (Url.Contains("youtu.be/"))
            {
                Split = Url.Split(new string[] { "youtu.be/" }, StringSplitOptions.None)[1];
            }

            if (Split.Length < 11)
            {
                Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.loadvideo.incorrect", Session.Langue));
                return;
            }
            UserRoom.LoaderVideoId = Split.Substring(0, 11);

            Session.SendWhisper(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.loadvideo", Session.Langue), UserRoom.LoaderVideoId));

        }
    }
}
