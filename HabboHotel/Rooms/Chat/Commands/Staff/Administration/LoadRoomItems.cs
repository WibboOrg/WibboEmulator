using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class LoadRoomItems : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            if (!int.TryParse(Params[1], out int RoomId))
            {
                return;
            }

            Room.GetRoomItemHandler().LoadFurniture(RoomId);
            Room.GetGameMap().GenerateMaps();
            UserRoom.SendWhisperChat("Mobi de l'appart n° " + RoomId + " chargé!");
            Session.SendPacket(new GetGuestRoomResultComposer(Session, Room.RoomData, false, true));
        }
    }
}
