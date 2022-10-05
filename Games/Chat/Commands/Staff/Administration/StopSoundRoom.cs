namespace WibboEmulator.Games.Chat.Commands.Cmd;

using WibboEmulator.Communication.Packets.Outgoing.Sound.SoundCustom;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class StopSoundRoom : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params) => Room.SendPacket(new StopSoundComposer((Params.Length != 2) ? "" : Params[1])); //Type = Trax
}
