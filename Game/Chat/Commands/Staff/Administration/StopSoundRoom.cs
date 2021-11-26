using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class StopSoundRoom : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.SendPacketWeb(new StopSoundComposer((Params.Length != 2) ? "" : Params[1])); //Type = Trax
        }
    }
}
