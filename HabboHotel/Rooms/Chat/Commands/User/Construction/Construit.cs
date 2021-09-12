using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Construit : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            if (Params.Length != 2)
            {
                return;
            }

            string Heigth = Params[1];            if (double.TryParse(Heigth, out double Result))            {                if (Result >= 0.01 && Result <= 10)                {                    UserRoom.ConstruitEnable = true;                    UserRoom.ConstruitHeigth = Result;                }            }        }    }}