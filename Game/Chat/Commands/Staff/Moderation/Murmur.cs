using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Murmur : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().ViewMurmur)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.true", Session.Langue));
            }
            else
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.murmur.false", Session.Langue));
            }

            Session.GetHabbo().ViewMurmur = !Session.GetHabbo().ViewMurmur;

        }
    }
}