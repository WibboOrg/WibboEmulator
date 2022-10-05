namespace WibboEmulator.Games.Chat.Commands.Cmd;

using WibboEmulator.Communication.Packets.Outgoing.Sound.SoundCustom;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class PlaySoundRoom : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length != 2)
        {
            return;
        }

        var SongName = Params[1];

        Room.SendPacket(new PlaySoundComposer(SongName, 1)); //Type = Trax
    }
}
