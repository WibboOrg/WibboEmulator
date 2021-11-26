using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class LoadRoomItems : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (!int.TryParse(Params[1], out int RoomId))
            {
                return;
            }

            if (Room.Id == RoomId)
            {
                return;
            }

            Room.GetRoomItemHandler().LoadFurniture(RoomId);
            Room.GetGameMap().GenerateMaps(true);
            UserRoom.SendWhisperChat("Mobi de l'appart n° " + RoomId + " chargé!");
            Session.SendPacket(new GetGuestRoomResultComposer(Session, Room.RoomData, false, true));
        }
    }
}
