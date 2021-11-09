using Butterfly.HabboHotel.GameClients;
namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class SetZ : IChatCommand    {        public string PermissionRequired
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

            string Heigth = Params[1];            if (!double.TryParse(Heigth, out double Result))
            {
                return;
            }

            if (Result < -100)
            {
                Result = 0;
            }

            if (Result > 100)
            {
                Result = 100;
            }

            UserRoom.ConstruitZMode = true;            UserRoom.ConstruitHeigth = Result;            UserRoom.SendWhisperChat("SetZ: " + Result);            if (Result >= 0)
            {
                Session.SendPacket(Room.GetGameMap().Model.setHeightMap((Result > 63) ? 63 : Result));
            }
        }    }}