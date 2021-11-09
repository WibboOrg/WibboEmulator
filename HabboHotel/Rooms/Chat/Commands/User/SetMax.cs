using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class SetMax : IChatCommand
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
            int.TryParse(Params[1], out int MaxUsers);
            if (MaxUsers > 10000 || MaxUsers <= 0)
            {
                Room.SetMaxUsers(75);
            }
            else
            {
                Room.SetMaxUsers(MaxUsers);
            }
        }
    }
}
