using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class SetSpeed : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            Room currentRoom = Session.GetHabbo().CurrentRoom;            if (currentRoom == null)
            {
                return;
            }

            if (!currentRoom.CheckRights(Session, true))
            {
                return;
            }

            try            {                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().SetSpeed(int.Parse(Params[1]));            }            catch            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));            }        }    }}