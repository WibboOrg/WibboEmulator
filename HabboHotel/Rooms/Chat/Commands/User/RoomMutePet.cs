using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class RoomMutePet : IChatCommand    {        public string PermissionRequired
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
        }        public void Execute(GameClients.GameClient Session, Room Room, string[] Params)        {            Room room = Session.GetHabbo().CurrentRoom;            if (room == null)
            {
                return;
            }

            if (room.RoomMutePets)            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.true", Session.Langue));                room.RoomMutePets = false;            }            else            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.false", Session.Langue));                room.RoomMutePets = true;            }        }    }}