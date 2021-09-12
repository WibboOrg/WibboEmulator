using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class GameTime : IChatCommand
    {
        public string PermissionRequired
        {
            get { return ""; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return ""; }
        }
        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)
        {
            if (ButterflyEnvironment.GetGame().GetAnimationManager().IsActivate())
            {
                string Time = ButterflyEnvironment.GetGame().GetAnimationManager().GetTime();
                UserRoom.SendWhisperChat("Prochaine animation de Jack & Daisy dans " + Time);
            }
            else
            {
                UserRoom.SendWhisperChat("Les animations de Jack & Daisy sont désactivées");
            }
        }
    }
}
