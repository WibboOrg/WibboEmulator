using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class StopSoundRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.SendPacketWeb(new StopSoundComposer((Params.Length != 2) ? "" : Params[1])); //Type = Trax
        }
    }
}
