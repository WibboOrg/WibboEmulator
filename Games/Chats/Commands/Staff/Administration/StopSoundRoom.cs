namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Communication.Packets.Outgoing.Sound;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class StopSoundRoom : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters) => room.SendPacket(new StopSoundComposer(parameters.Length != 2 ? "" : parameters[1])); //Type = Trax
}
