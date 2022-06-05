
using Wibbo.Communication.Packets.Outgoing.Sound.SoundCustom;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class PlaySoundRoom : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string SongName = Params[1];

            Room.SendPacket(new PlaySoundComposer(SongName, 1)); //Type = Trax
        }
    }
}
