using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class VipProtect : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetHabbo().PremiumProtect = !Session.GetHabbo().PremiumProtect;

            if (Session.GetHabbo().PremiumProtect)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.premium.true", Session.Langue));
            }
            else
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.premium.false", Session.Langue));
            }
        }
    }
}
