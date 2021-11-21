using Butterfly.Game.Clients;namespace Butterfly.Game.Rooms.Chat.Commands.Cmd{    internal class SetSpeed : IChatCommand    {        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)        {            Room currentRoom = Session.GetHabbo().CurrentRoom;            if (currentRoom == null)
            {
                return;
            }

            if (!currentRoom.CheckRights(Session, true))
            {
                return;
            }

            try            {                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().SetSpeed(int.Parse(Params[1]));            }            catch            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.intonly", Session.Langue));            }        }    }}