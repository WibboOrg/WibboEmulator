
using WibboEmulator.Communication.Packets.Outgoing.Sound.SoundCustom;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class StopSoundRoom : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.SendPacket(new StopSoundComposer((Params.Length != 2) ? "" : Params[1])); //Type = Trax
        }
    }
}
