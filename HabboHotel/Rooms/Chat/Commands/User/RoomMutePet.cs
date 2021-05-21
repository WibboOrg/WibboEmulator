using Butterfly.HabboHotel.GameClients;namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class RoomMutePet : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            Room room = Session.GetHabbo().CurrentRoom;            if (room == null)
            {
                return;
            }

            if (room.RoomMutePets)            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.true", Session.Langue));                room.RoomMutePets = false;            }            else            {                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.roommutepet.false", Session.Langue));                room.RoomMutePets = true;            }        }    }}