
using Wibbo.Communication.Packets.Outgoing.Sound.SoundCustom;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class StopSoundRoom : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.SendPacket(new StopSoundComposer((Params.Length != 2) ? "" : Params[1])); //Type = Trax
        }
    }
}
