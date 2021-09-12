using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Use : IChatCommand
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
            if (Params.Length != 2)
            {
                return;
            }

            string Count = Params[1];
            if (!int.TryParse(Count, out int UseCount))
            {
                return;
            }

            if (UseCount < 0 || UseCount > 100)
            {
                return;
            }

            Session.GetHabbo().forceUse = UseCount;
        }
    }
}
